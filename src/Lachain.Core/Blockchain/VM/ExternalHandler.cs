﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Lachain.Core.Blockchain.Error;
using Lachain.Core.Blockchain.VM.ExecutionFrame;
using Lachain.Crypto;
using Lachain.Proto;
using Lachain.Utility;
using Lachain.Utility.Serialization;
using Lachain.Utility.Utils;
using WebAssembly.Runtime;

namespace Lachain.Core.Blockchain.VM
{
    public class ExternalHandler : IExternalHandler
    {
        private const string EnvModule = "env";

        private static InvocationResult DoInternalCall(
            UInt160 caller,
            UInt160 address,
            byte[] input,
            ulong gasLimit)
        {
            var currentFrame = VirtualMachine.ExecutionFrames.Peek();
            var context = currentFrame.InvocationContext.NextContext(caller);
            return ContractInvoker.Invoke(address, context, input, gasLimit);
        }

        private static byte[]? SafeCopyFromMemory(UnmanagedMemory memory, int offset, int length)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek();
            if (length < 0 || offset < 0)
                return null;
            if (offset + length > memory.Size)
                return null;
            frame.UseGas(GasMetering.CopyFromMemoryGasPerByte * (ulong) length);
            var buffer = new byte[length];
            try
            {
                Marshal.Copy(IntPtr.Add(memory.Start, offset), buffer, 0, length);
            }
            catch (ArgumentNullException)
            {
                return null;
            }

            return buffer;
        }

        private static bool SafeCopyToMemory(UnmanagedMemory memory, byte[] data, int offset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek();
            if (offset < 0 || offset + data.Length > memory.Size)
                return false;
            frame.UseGas(GasMetering.CopyToMemoryGasPerByte * (ulong) data.Length);
            try
            {
                Marshal.Copy(data, 0, IntPtr.Add(memory.Start, offset), data.Length);
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        public static int Handler_Env_GetCallValue(int offset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek();
            frame.UseGas(GasMetering.GetCallValueGasCost);
            if (offset < 0 || offset >= frame.Input.Length)
                throw new InvalidContractException("Bad getcallvalue call");
            return frame.Input[offset];
        }

        public static int Handler_Env_GetCallSize()
        {
            var frame = VirtualMachine.ExecutionFrames.Peek();
            frame.UseGas(GasMetering.GetCallSizeGasCost);
            return frame.Input.Length;
        }

        public static void Handler_Env_CopyCallValue(int from, int to, int offset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call COPYCALLVALUE outside wasm frame");
            if (from < 0 || to > frame.Input.Length || from > to)
                throw new InvalidContractException("Copy to contract memory failed: bad range");
            if (!SafeCopyToMemory(frame.Memory, frame.Input.Skip(from).Take(to - from).ToArray(), offset))
                throw new InvalidContractException("Copy to contract memory failed");
        }

        public static void Handler_Env_WriteLog(int offset, int length)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call WRITELOG outside wasm frame");
            var buffer = SafeCopyFromMemory(frame.Memory, offset, length);
            if (buffer == null)
                throw new InvalidContractException("Bad call to WRITELOG");
        }

        public static int Handler_Env_InvokeContract(
            int callSignatureOffset, int inputLength, int inputOffset, int valueOffset, int gasOffset,
            int returnValueOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call INVOKECONTRACT outside wasm frame");
            var snapshot = frame.InvocationContext.Snapshot;
            var addressBuffer = SafeCopyFromMemory(frame.Memory, callSignatureOffset, 20);
            var inputBuffer = SafeCopyFromMemory(frame.Memory, inputOffset, inputLength);
            if (addressBuffer is null || inputBuffer is null)
                throw new InvalidContractException("Bad call to call function");
            var address = addressBuffer.Take(20).ToArray().ToUInt160();
            var value = SafeCopyFromMemory(frame.Memory, valueOffset, 32)?.ToUInt256().ToMoney();
            if (value is null)
                throw new InvalidContractException("Bad call to call function");
            if (value > Money.Zero)
            {
                frame.UseGas(GasMetering.TransferFundsGasCost);
                var result = snapshot.Balances.TransferBalance(frame.CurrentAddress, address, value);
                if (!result)
                    throw new InsufficientFundsException();
            }

            var gasBuffer = SafeCopyFromMemory(frame.Memory, gasOffset, 8);
            if (gasBuffer is null)
                throw new InvalidContractException("Bad call to call function");
            var gasLimit = gasBuffer.AsReadOnlySpan().ToUInt64();
            if (gasLimit == 0 || gasLimit > frame.GasLimit - frame.GasUsed)
                gasLimit = frame.GasLimit - frame.GasUsed;
            var callResult = DoInternalCall(frame.CurrentAddress, address, inputBuffer, gasLimit);
            if (callResult.Status != ExecutionStatus.Ok)
                throw new InvalidContractException("Cannot invoke call: " + callResult.Status);
            frame.UseGas(callResult.GasUsed);
            if (!SafeCopyToMemory(frame.Memory, callResult.ReturnValue ?? Array.Empty<byte>(), returnValueOffset))
                throw new InvalidContractException("Cannot invoke call: cannot pass return value");
            return 0;
        }

        public static void Handler_Env_LoadStorage(int keyOffset, int valueOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call LOADSTORAGE outside wasm frame");
            frame.UseGas(GasMetering.LoadStorageGasCost);
            var key = SafeCopyFromMemory(frame.Memory, keyOffset, 32);
            if (key is null) throw new InvalidContractException("Bad call to LOADSTORAGE");
            if (key.Length < 32)
                key = _AlignTo32(key);
            var value = frame.InvocationContext.Snapshot.Storage.GetValue(frame.CurrentAddress, key.ToUInt256());
            if (!SafeCopyToMemory(frame.Memory, value.ToBytes(), valueOffset))
                throw new InvalidContractException("Cannot copy storageload result to memory");
        }

        public static void Handler_Env_SaveStorage(int keyOffset, int valueOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call SAVESTORAGE outside wasm frame");
            frame.UseGas(GasMetering.SaveStorageGasCost);
            var key = SafeCopyFromMemory(frame.Memory, keyOffset, 32);
            if (key is null)
                throw new InvalidContractException("Bad call to SAVESTORAGE");
            if (key.Length < 32)
                key = _AlignTo32(key);
            var value = SafeCopyFromMemory(frame.Memory, valueOffset, 32);
            if (value is null) throw new InvalidContractException("Bad call to SAVESTORAGE");
            frame.InvocationContext.Snapshot.Storage.SetValue(
                frame.CurrentAddress, key.ToUInt256(), value.ToUInt256()
            );
        }

        private static byte[] _AlignTo32(byte[] buffer)
        {
            if (buffer.Length == 32)
                return buffer;
            var result = new byte[32];
            Array.Copy(buffer, 0, result, 0, buffer.Length);
            return result;
        }

        public static void Handler_Env_SetReturn(int offset, int length)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call SETRETURN outside wasm frame");
            var ret = SafeCopyFromMemory(frame.Memory, offset, length);
            if (ret is null)
                throw new InvalidContractException("Bad call to SETRETURN");
            frame.ReturnValue = ret;
        }

        public static void Handler_Env_GetSender(int dataOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call GETSENDER outside wasm frame");
            var data = frame.InvocationContext.Sender.ToBytes();
            var ret = SafeCopyToMemory(frame.Memory, data, dataOffset);
            if (!ret)
                throw new InvalidContractException("Bad call to GETSENDER");
        }

        public static void Handler_Env_SystemHalt(int haltCode)
        {
            throw new HaltException(haltCode);
        }

        public static void Handler_Env_CryptoKeccak256(int dataOffset, int dataLength, int resultOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call KECCAK256 outside wasm frame");
            frame.UseGas(GasMetering.Keccak256GasCost + GasMetering.Keccak256GasPerByte * (ulong) dataLength);
            var data = SafeCopyFromMemory(frame.Memory, dataOffset, dataLength) ??
                       throw new InvalidOperationException();
            var result = data.KeccakBytes();
            SafeCopyToMemory(frame.Memory, result, resultOffset);
        }

        public static void Handler_Env_CryptoSha256(int dataOffset, int dataLength, int resultOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call SHA256 outside wasm frame");
            frame.UseGas(GasMetering.Sha256GasGasCost + GasMetering.Sha256GasPerByte * (ulong) dataLength);
            var data = SafeCopyFromMemory(frame.Memory, dataOffset, dataLength) ??
                       throw new InvalidOperationException();
            var result = data.Sha256Bytes();
            SafeCopyToMemory(frame.Memory, result, resultOffset);
        }

        public static void Handler_Env_CryptoRipemd160(int dataOffset, int dataLength, int resultOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call RIPEMD outside wasm frame");
            frame.UseGas(GasMetering.Ripemd160GasCost + GasMetering.Ripemd160GasPerByte * (ulong) dataLength);
            var data = SafeCopyFromMemory(frame.Memory, dataOffset, dataLength) ??
                       throw new InvalidOperationException();
            var result = data.RipemdBytes();
            SafeCopyToMemory(frame.Memory, result, resultOffset);
        }

        public static void Handler_Env_CryptoRecover(int messageOffset, int messageLength, int signatureOffset,
            int resultOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call ECRECOVER outside wasm frame");
            frame.UseGas(GasMetering.RecoverGasCost);
            var message = SafeCopyFromMemory(frame.Memory, messageOffset, messageLength) ??
                          throw new InvalidOperationException();
            var sig = SafeCopyFromMemory(frame.Memory, signatureOffset, SignatureUtils.Length) ??
                      throw new InvalidOperationException();
            var publicKey = VirtualMachine.Crypto.RecoverSignature(message, sig);
            SafeCopyToMemory(frame.Memory, publicKey, resultOffset);
        }

        public static void Handler_Env_CryptoVerify(int messageOffset, int messageLength, int signatureOffset,
            int publicKeyOffset, int resultOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call ECVERIFY outside wasm frame");
            frame.UseGas(GasMetering.VerifyGasCost);
            var message = SafeCopyFromMemory(frame.Memory, messageOffset, messageLength) ??
                          throw new InvalidOperationException();
            var sig = SafeCopyFromMemory(frame.Memory, signatureOffset, SignatureUtils.Length) ??
                      throw new InvalidOperationException();
            var publicKey = SafeCopyFromMemory(frame.Memory, publicKeyOffset, CryptoUtils.PublicKeyLength) ??
                            throw new InvalidOperationException();
            var result = VirtualMachine.Crypto.VerifySignature(message, sig, publicKey);
            SafeCopyToMemory(frame.Memory, new[] {result ? (byte) 1 : (byte) 0}, resultOffset);
        }

        public static void Handler_Env_GetTransferredFunds(int dataOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call GETCALLVALUE outside wasm frame");
            var data = frame.InvocationContext.Value.ToBytes();
            var ret = SafeCopyToMemory(frame.Memory, data, dataOffset);
            if (!ret)
                throw new InvalidContractException("Bad call to (get_transferred_funds)");
        }

        public static void Handler_Env_GetTransactionHash(int dataOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call TXHASH outside wasm frame");
            var data = frame.InvocationContext.TransactionHash.ToBytes();
            var ret = SafeCopyToMemory(frame.Memory, data, dataOffset);
            if (!ret)
                throw new InvalidContractException("Bad call to (get_transaction_hash)");
        }

        public static void Handle_Env_WriteEvent(int signatureOffset, int valueOffset, int valueLength)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call WRITEEVENT outside wasm frame");
            frame.UseGas(GasMetering.WriteEventPerByteGas * (uint) (valueLength + 32));
            var signature = SafeCopyFromMemory(frame.Memory, signatureOffset, 32) ??
                            throw new InvalidOperationException();
            var value = SafeCopyFromMemory(frame.Memory, valueOffset, valueLength) ??
                        throw new InvalidOperationException();
            var ev = new Event
            {
                Contract = frame.CurrentAddress,
                Data = ByteString.CopyFrom(value),
                TransactionHash = frame.InvocationContext.TransactionHash,
                Index = 0, /* will be replaced in (IEventSnapshot::AddEvent) method */
                SignatureHash = signature.ToUInt256()
            };
            frame.InvocationContext.Snapshot.Events.AddEvent(ev);
        }
        
        public static void Handler_Env_GetAddress(int resultOffset)
        {
            var frame = VirtualMachine.ExecutionFrames.Peek() as WasmExecutionFrame
                        ?? throw new InvalidOperationException("Cannot call GetAddress outside wasm frame");
            var result = (frame.CurrentAddress).ToBytes();
            SafeCopyToMemory(frame.Memory, result, resultOffset);
        }

        private static FunctionImport CreateImport(string methodName)
        {
            var methodInfo = typeof(ExternalHandler).GetMethod(methodName) ?? throw new ArgumentNullException();
            var isAction = methodInfo.ReturnType == typeof(void);
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                return new FunctionImport(
                    Delegate.CreateDelegate(Expression.GetActionType(types.ToArray()), methodInfo)
                );
            }

            types = types.Concat(new[] {methodInfo.ReturnType});
            return new FunctionImport(Delegate.CreateDelegate(Expression.GetFuncType(types.ToArray()), methodInfo));
        }

        public ImportDictionary GetFunctionImports()
        {
            var result = new ImportDictionary
            {
                {EnvModule, "get_call_value", CreateImport(nameof(Handler_Env_GetCallValue))},
                {EnvModule, "get_call_size", CreateImport(nameof(Handler_Env_GetCallSize))},
                {EnvModule, "copy_call_value", CreateImport(nameof(Handler_Env_CopyCallValue))},
                {EnvModule, "invoke_contract", CreateImport(nameof(Handler_Env_InvokeContract))},
                {EnvModule, "write_log", CreateImport(nameof(Handler_Env_WriteLog))},
                {EnvModule, "load_storage", CreateImport(nameof(Handler_Env_LoadStorage))},
                {EnvModule, "save_storage", CreateImport(nameof(Handler_Env_SaveStorage))},
                {EnvModule, "set_return", CreateImport(nameof(Handler_Env_SetReturn))},
                {EnvModule, "get_sender", CreateImport(nameof(Handler_Env_GetSender))},
                {EnvModule, "system_halt", CreateImport(nameof(Handler_Env_SystemHalt))},
                {EnvModule, "get_transferred_funds", CreateImport(nameof(Handler_Env_GetTransferredFunds))},
                {EnvModule, "get_transaction_hash", CreateImport(nameof(Handler_Env_GetTransactionHash))},
                {EnvModule, "write_event", CreateImport(nameof(Handle_Env_WriteEvent))},
                {EnvModule, "get_address", CreateImport(nameof(Handler_Env_GetAddress))},
                // /* crypto hash bindings */
                {EnvModule, "crypto_keccak256", CreateImport(nameof(Handler_Env_CryptoKeccak256))},
                {EnvModule, "crypto_sha256", CreateImport(nameof(Handler_Env_CryptoSha256))},
                {EnvModule, "crypto_ripemd160", CreateImport(nameof(Handler_Env_CryptoRipemd160))},
                // /* cryptography methods */
                {EnvModule, "crypto_recover", CreateImport(nameof(Handler_Env_CryptoRecover))},
                {EnvModule, "crypto_verify", CreateImport(nameof(Handler_Env_CryptoVerify))},
                /* memory methods */
            };
            return result;
        }
    }
}