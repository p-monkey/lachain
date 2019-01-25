﻿using System;
using System.Collections.Generic;
using System.IO;
using Phorkus.Proto;
using Phorkus.Storage.State;
using Phorkus.WebAssembly;

namespace Phorkus.Core.VM
{
    public class VirtualMachine : IVirtualMachine
    {
        public static Stack<ExecutionFrame> ExecutionFrames { get; } = new Stack<ExecutionFrame>();
        public static IBlockchainSnapshot BlockchainSnapshot { get; set; }

        public VirtualMachine(IStateManager stateManager)
        {
            BlockchainSnapshot = stateManager.LastApprovedSnapshot;
        }

        // TODO: protection from multiple instantiation

        public bool VerifyContract(Contract contract)
        {
            var contractCode = contract.Wasm.ToByteArray();
            try
            {
                using (var stream = new MemoryStream(contractCode))
                    Compile.FromBinary<dynamic>(stream);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            return false;
        }

        public ExecutionStatus InvokeContract(Contract contract, Invocation invocation)
        {
            try
            {
                return _InvokeContractUnsafe(contract, invocation);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return ExecutionStatus.UNKNOWN_ERROR;
            }
        }

        private static ExecutionStatus _InvokeContractUnsafe(Contract contract, Invocation invocation)
        {
            if (ExecutionFrames.Count != 0)
                return ExecutionStatus.VM_CORRUPTION;
            if (contract.Version != ContractVersion.Wasm)
                return ExecutionStatus.INCOMPATIBLE_CODE;
            
            var status = ExecutionFrame.FromInvocation(contract.Wasm.ToByteArray(), invocation, new DefaultBlockchainInterface(), out var rootFrame);
            if (status != ExecutionStatus.OK) return status;
            ExecutionFrames.Push(rootFrame);
            return rootFrame.Execute();
        }
    }
}