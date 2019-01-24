// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: contract.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Phorkus.Proto {

  /// <summary>Holder for reflection information generated from contract.proto</summary>
  public static partial class ContractReflection {

    #region Descriptor
    /// <summary>File descriptor for contract.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ContractReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg5jb250cmFjdC5wcm90bxoNZGVmYXVsdC5wcm90byKAAQoLQ29udHJhY3RB",
            "QkkSDgoGbWV0aG9kGAEgASgJEhwKBWlucHV0GAIgAygOMg0uQ29udHJhY3RU",
            "eXBlEh0KBm91dHB1dBgDIAEoDjINLkNvbnRyYWN0VHlwZRIkCgltb2RpZmll",
            "cnMYBCADKA4yES5Db250cmFjdE1vZGlmaWVyIm4KCENvbnRyYWN0EhYKBGhh",
            "c2gYASABKAsyCC5VSW50MTYwEhkKA2FiaRgCIAMoCzIMLkNvbnRyYWN0QUJJ",
            "EgwKBHdhc20YAyABKAwSIQoHdmVyc2lvbhgEIAEoDjIQLkNvbnRyYWN0VmVy",
            "c2lvbiIpCg5Db250cmFjdEdsb2JhbBIXCg90b3RhbF9jb250cmFjdHMYASAB",
            "KA0ibgoKSW52b2NhdGlvbhIiChBjb250cmFjdF9hZGRyZXNzGAEgASgLMggu",
            "VUludDE2MBITCgttZXRob2RfbmFtZRgCIAEoCRINCgVpbnB1dBgDIAEoDBIY",
            "CgZzZW5kZXIYBCABKAsyCC5VSW50MTYwKiwKD0NvbnRyYWN0VmVyc2lvbhIZ",
            "ChVDT05UUkFDVF9WRVJTSU9OX1dBU00QACrPAgoMQ29udHJhY3RUeXBlEhsK",
            "F0NPTlRSQUNUX1RZUEVfU0lHTkFUVVJFEAASGQoVQ09OVFJBQ1RfVFlQRV9C",
            "T09MRUFOEAESGQoVQ09OVFJBQ1RfVFlQRV9JTlRFR0VSEAISFgoSQ09OVFJB",
            "Q1RfVFlQRV9MT05HEAMSGAoUQ09OVFJBQ1RfVFlQRV9JTlQxNjAQBBIYChRD",
            "T05UUkFDVF9UWVBFX0lOVDI1NhAFEhwKGENPTlRSQUNUX1RZUEVfQllURV9B",
            "UlJBWRAGEhwKGENPTlRSQUNUX1RZUEVfUFVCTElDX0tFWRAHEhgKFENPTlRS",
            "QUNUX1RZUEVfU1RSSU5HEAgSFwoTQ09OVFJBQ1RfVFlQRV9BUlJBWRAJEhkK",
            "FUNPTlRSQUNUX1RZUEVfQUREUkVTUxAKEhYKEkNPTlRSQUNUX1RZUEVfVk9J",
            "RBALKjEKEENvbnRyYWN0TW9kaWZpZXISHQoZQ09OVFJBQ1RfTU9ESUZJRVJf",
            "UEFZQUJMRRAAQhCqAg1QaG9ya3VzLlByb3RvYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Phorkus.Proto.DefaultReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Phorkus.Proto.ContractVersion), typeof(global::Phorkus.Proto.ContractType), typeof(global::Phorkus.Proto.ContractModifier), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.ContractABI), global::Phorkus.Proto.ContractABI.Parser, new[]{ "Method", "Input", "Output", "Modifiers" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.Contract), global::Phorkus.Proto.Contract.Parser, new[]{ "Hash", "Abi", "Wasm", "Version" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.ContractGlobal), global::Phorkus.Proto.ContractGlobal.Parser, new[]{ "TotalContracts" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.Invocation), global::Phorkus.Proto.Invocation.Parser, new[]{ "ContractAddress", "MethodName", "Input", "Sender" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum ContractVersion {
    [pbr::OriginalName("CONTRACT_VERSION_WASM")] Wasm = 0,
  }

  public enum ContractType {
    [pbr::OriginalName("CONTRACT_TYPE_SIGNATURE")] Signature = 0,
    [pbr::OriginalName("CONTRACT_TYPE_BOOLEAN")] Boolean = 1,
    [pbr::OriginalName("CONTRACT_TYPE_INTEGER")] Integer = 2,
    [pbr::OriginalName("CONTRACT_TYPE_LONG")] Long = 3,
    [pbr::OriginalName("CONTRACT_TYPE_INT160")] Int160 = 4,
    [pbr::OriginalName("CONTRACT_TYPE_INT256")] Int256 = 5,
    [pbr::OriginalName("CONTRACT_TYPE_BYTE_ARRAY")] ByteArray = 6,
    [pbr::OriginalName("CONTRACT_TYPE_PUBLIC_KEY")] PublicKey = 7,
    [pbr::OriginalName("CONTRACT_TYPE_STRING")] String = 8,
    [pbr::OriginalName("CONTRACT_TYPE_ARRAY")] Array = 9,
    [pbr::OriginalName("CONTRACT_TYPE_ADDRESS")] Address = 10,
    [pbr::OriginalName("CONTRACT_TYPE_VOID")] Void = 11,
  }

  public enum ContractModifier {
    [pbr::OriginalName("CONTRACT_MODIFIER_PAYABLE")] Payable = 0,
  }

  #endregion

  #region Messages
  public sealed partial class ContractABI : pb::IMessage<ContractABI> {
    private static readonly pb::MessageParser<ContractABI> _parser = new pb::MessageParser<ContractABI>(() => new ContractABI());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ContractABI> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ContractReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ContractABI() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ContractABI(ContractABI other) : this() {
      method_ = other.method_;
      input_ = other.input_.Clone();
      output_ = other.output_;
      modifiers_ = other.modifiers_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ContractABI Clone() {
      return new ContractABI(this);
    }

    /// <summary>Field number for the "method" field.</summary>
    public const int MethodFieldNumber = 1;
    private string method_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Method {
      get { return method_; }
      set {
        method_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "input" field.</summary>
    public const int InputFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Phorkus.Proto.ContractType> _repeated_input_codec
        = pb::FieldCodec.ForEnum(18, x => (int) x, x => (global::Phorkus.Proto.ContractType) x);
    private readonly pbc::RepeatedField<global::Phorkus.Proto.ContractType> input_ = new pbc::RepeatedField<global::Phorkus.Proto.ContractType>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Phorkus.Proto.ContractType> Input {
      get { return input_; }
    }

    /// <summary>Field number for the "output" field.</summary>
    public const int OutputFieldNumber = 3;
    private global::Phorkus.Proto.ContractType output_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.ContractType Output {
      get { return output_; }
      set {
        output_ = value;
      }
    }

    /// <summary>Field number for the "modifiers" field.</summary>
    public const int ModifiersFieldNumber = 4;
    private static readonly pb::FieldCodec<global::Phorkus.Proto.ContractModifier> _repeated_modifiers_codec
        = pb::FieldCodec.ForEnum(34, x => (int) x, x => (global::Phorkus.Proto.ContractModifier) x);
    private readonly pbc::RepeatedField<global::Phorkus.Proto.ContractModifier> modifiers_ = new pbc::RepeatedField<global::Phorkus.Proto.ContractModifier>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Phorkus.Proto.ContractModifier> Modifiers {
      get { return modifiers_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ContractABI);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ContractABI other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Method != other.Method) return false;
      if(!input_.Equals(other.input_)) return false;
      if (Output != other.Output) return false;
      if(!modifiers_.Equals(other.modifiers_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Method.Length != 0) hash ^= Method.GetHashCode();
      hash ^= input_.GetHashCode();
      if (Output != 0) hash ^= Output.GetHashCode();
      hash ^= modifiers_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Method.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Method);
      }
      input_.WriteTo(output, _repeated_input_codec);
      if (Output != 0) {
        output.WriteRawTag(24);
        output.WriteEnum((int) Output);
      }
      modifiers_.WriteTo(output, _repeated_modifiers_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Method.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Method);
      }
      size += input_.CalculateSize(_repeated_input_codec);
      if (Output != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Output);
      }
      size += modifiers_.CalculateSize(_repeated_modifiers_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ContractABI other) {
      if (other == null) {
        return;
      }
      if (other.Method.Length != 0) {
        Method = other.Method;
      }
      input_.Add(other.input_);
      if (other.Output != 0) {
        Output = other.Output;
      }
      modifiers_.Add(other.modifiers_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Method = input.ReadString();
            break;
          }
          case 18:
          case 16: {
            input_.AddEntriesFrom(input, _repeated_input_codec);
            break;
          }
          case 24: {
            output_ = (global::Phorkus.Proto.ContractType) input.ReadEnum();
            break;
          }
          case 34:
          case 32: {
            modifiers_.AddEntriesFrom(input, _repeated_modifiers_codec);
            break;
          }
        }
      }
    }

  }

  public sealed partial class Contract : pb::IMessage<Contract> {
    private static readonly pb::MessageParser<Contract> _parser = new pb::MessageParser<Contract>(() => new Contract());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Contract> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ContractReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Contract() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Contract(Contract other) : this() {
      hash_ = other.hash_ != null ? other.hash_.Clone() : null;
      abi_ = other.abi_.Clone();
      wasm_ = other.wasm_;
      version_ = other.version_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Contract Clone() {
      return new Contract(this);
    }

    /// <summary>Field number for the "hash" field.</summary>
    public const int HashFieldNumber = 1;
    private global::Phorkus.Proto.UInt160 hash_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.UInt160 Hash {
      get { return hash_; }
      set {
        hash_ = value;
      }
    }

    /// <summary>Field number for the "abi" field.</summary>
    public const int AbiFieldNumber = 2;
    private static readonly pb::FieldCodec<global::Phorkus.Proto.ContractABI> _repeated_abi_codec
        = pb::FieldCodec.ForMessage(18, global::Phorkus.Proto.ContractABI.Parser);
    private readonly pbc::RepeatedField<global::Phorkus.Proto.ContractABI> abi_ = new pbc::RepeatedField<global::Phorkus.Proto.ContractABI>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Phorkus.Proto.ContractABI> Abi {
      get { return abi_; }
    }

    /// <summary>Field number for the "wasm" field.</summary>
    public const int WasmFieldNumber = 3;
    private pb::ByteString wasm_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Wasm {
      get { return wasm_; }
      set {
        wasm_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "version" field.</summary>
    public const int VersionFieldNumber = 4;
    private global::Phorkus.Proto.ContractVersion version_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.ContractVersion Version {
      get { return version_; }
      set {
        version_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Contract);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Contract other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Hash, other.Hash)) return false;
      if(!abi_.Equals(other.abi_)) return false;
      if (Wasm != other.Wasm) return false;
      if (Version != other.Version) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (hash_ != null) hash ^= Hash.GetHashCode();
      hash ^= abi_.GetHashCode();
      if (Wasm.Length != 0) hash ^= Wasm.GetHashCode();
      if (Version != 0) hash ^= Version.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (hash_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Hash);
      }
      abi_.WriteTo(output, _repeated_abi_codec);
      if (Wasm.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Wasm);
      }
      if (Version != 0) {
        output.WriteRawTag(32);
        output.WriteEnum((int) Version);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (hash_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Hash);
      }
      size += abi_.CalculateSize(_repeated_abi_codec);
      if (Wasm.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Wasm);
      }
      if (Version != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Version);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Contract other) {
      if (other == null) {
        return;
      }
      if (other.hash_ != null) {
        if (hash_ == null) {
          hash_ = new global::Phorkus.Proto.UInt160();
        }
        Hash.MergeFrom(other.Hash);
      }
      abi_.Add(other.abi_);
      if (other.Wasm.Length != 0) {
        Wasm = other.Wasm;
      }
      if (other.Version != 0) {
        Version = other.Version;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (hash_ == null) {
              hash_ = new global::Phorkus.Proto.UInt160();
            }
            input.ReadMessage(hash_);
            break;
          }
          case 18: {
            abi_.AddEntriesFrom(input, _repeated_abi_codec);
            break;
          }
          case 26: {
            Wasm = input.ReadBytes();
            break;
          }
          case 32: {
            version_ = (global::Phorkus.Proto.ContractVersion) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  public sealed partial class ContractGlobal : pb::IMessage<ContractGlobal> {
    private static readonly pb::MessageParser<ContractGlobal> _parser = new pb::MessageParser<ContractGlobal>(() => new ContractGlobal());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ContractGlobal> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ContractReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ContractGlobal() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ContractGlobal(ContractGlobal other) : this() {
      totalContracts_ = other.totalContracts_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ContractGlobal Clone() {
      return new ContractGlobal(this);
    }

    /// <summary>Field number for the "total_contracts" field.</summary>
    public const int TotalContractsFieldNumber = 1;
    private uint totalContracts_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint TotalContracts {
      get { return totalContracts_; }
      set {
        totalContracts_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ContractGlobal);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ContractGlobal other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (TotalContracts != other.TotalContracts) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (TotalContracts != 0) hash ^= TotalContracts.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (TotalContracts != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(TotalContracts);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (TotalContracts != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(TotalContracts);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ContractGlobal other) {
      if (other == null) {
        return;
      }
      if (other.TotalContracts != 0) {
        TotalContracts = other.TotalContracts;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            TotalContracts = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Invocation : pb::IMessage<Invocation> {
    private static readonly pb::MessageParser<Invocation> _parser = new pb::MessageParser<Invocation>(() => new Invocation());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Invocation> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ContractReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Invocation() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Invocation(Invocation other) : this() {
      contractAddress_ = other.contractAddress_ != null ? other.contractAddress_.Clone() : null;
      methodName_ = other.methodName_;
      input_ = other.input_;
      sender_ = other.sender_ != null ? other.sender_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Invocation Clone() {
      return new Invocation(this);
    }

    /// <summary>Field number for the "contract_address" field.</summary>
    public const int ContractAddressFieldNumber = 1;
    private global::Phorkus.Proto.UInt160 contractAddress_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.UInt160 ContractAddress {
      get { return contractAddress_; }
      set {
        contractAddress_ = value;
      }
    }

    /// <summary>Field number for the "method_name" field.</summary>
    public const int MethodNameFieldNumber = 2;
    private string methodName_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string MethodName {
      get { return methodName_; }
      set {
        methodName_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "input" field.</summary>
    public const int InputFieldNumber = 3;
    private pb::ByteString input_ = pb::ByteString.Empty;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pb::ByteString Input {
      get { return input_; }
      set {
        input_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "sender" field.</summary>
    public const int SenderFieldNumber = 4;
    private global::Phorkus.Proto.UInt160 sender_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.UInt160 Sender {
      get { return sender_; }
      set {
        sender_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Invocation);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Invocation other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(ContractAddress, other.ContractAddress)) return false;
      if (MethodName != other.MethodName) return false;
      if (Input != other.Input) return false;
      if (!object.Equals(Sender, other.Sender)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (contractAddress_ != null) hash ^= ContractAddress.GetHashCode();
      if (MethodName.Length != 0) hash ^= MethodName.GetHashCode();
      if (Input.Length != 0) hash ^= Input.GetHashCode();
      if (sender_ != null) hash ^= Sender.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (contractAddress_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(ContractAddress);
      }
      if (MethodName.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(MethodName);
      }
      if (Input.Length != 0) {
        output.WriteRawTag(26);
        output.WriteBytes(Input);
      }
      if (sender_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Sender);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (contractAddress_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ContractAddress);
      }
      if (MethodName.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(MethodName);
      }
      if (Input.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeBytesSize(Input);
      }
      if (sender_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Sender);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Invocation other) {
      if (other == null) {
        return;
      }
      if (other.contractAddress_ != null) {
        if (contractAddress_ == null) {
          contractAddress_ = new global::Phorkus.Proto.UInt160();
        }
        ContractAddress.MergeFrom(other.ContractAddress);
      }
      if (other.MethodName.Length != 0) {
        MethodName = other.MethodName;
      }
      if (other.Input.Length != 0) {
        Input = other.Input;
      }
      if (other.sender_ != null) {
        if (sender_ == null) {
          sender_ = new global::Phorkus.Proto.UInt160();
        }
        Sender.MergeFrom(other.Sender);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (contractAddress_ == null) {
              contractAddress_ = new global::Phorkus.Proto.UInt160();
            }
            input.ReadMessage(contractAddress_);
            break;
          }
          case 18: {
            MethodName = input.ReadString();
            break;
          }
          case 26: {
            Input = input.ReadBytes();
            break;
          }
          case 34: {
            if (sender_ == null) {
              sender_ = new global::Phorkus.Proto.UInt160();
            }
            input.ReadMessage(sender_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
