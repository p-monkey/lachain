// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: consensus.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Phorkus.Proto {

  /// <summary>Holder for reflection information generated from consensus.proto</summary>
  public static partial class ConsensusReflection {

    #region Descriptor
    /// <summary>File descriptor for consensus.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ConsensusReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg9jb25zZW5zdXMucHJvdG8aDWRlZmF1bHQucHJvdG8aEXRyYW5zYWN0aW9u",
            "LnByb3RvIo8BCglWYWxpZGF0b3ISDwoHdmVyc2lvbhgBIAEoDRIbCglwcmV2",
            "X2hhc2gYAiABKAsyCC5VSW50MjU2EhMKC2Jsb2NrX2luZGV4GAMgASgEEhcK",
            "D3ZhbGlkYXRvcl9pbmRleBgEIAEoDRIRCgl0aW1lc3RhbXAYBSABKAQSEwoL",
            "dmlld19udW1iZXIYBiABKA0iSwoRQ2hhbmdlVmlld1JlcXVlc3QSHQoJdmFs",
            "aWRhdG9yGAEgASgLMgouVmFsaWRhdG9yEhcKD25ld192aWV3X251bWJlchgC",
            "IAEoDSLEAQoTQmxvY2tQcmVwYXJlUmVxdWVzdBIdCgl2YWxpZGF0b3IYASAB",
            "KAsyCi5WYWxpZGF0b3ISDQoFbm9uY2UYAiABKAQSJAoSdHJhbnNhY3Rpb25f",
            "aGFzaGVzGAMgAygLMgguVUludDI1NhInChFtaW5lcl90cmFuc2FjdGlvbhgE",
            "IAEoCzIMLlRyYW5zYWN0aW9uEhEKCXRpbWVzdGFtcBgFIAEoBBIdCglzaWdu",
            "YXR1cmUYBiABKAsyCi5TaWduYXR1cmUiUQoRQmxvY2tQcmVwYXJlUmVwbHkS",
            "HQoJdmFsaWRhdG9yGAEgASgLMgouVmFsaWRhdG9yEh0KCXNpZ25hdHVyZRgC",
            "IAEoCzIKLlNpZ25hdHVyZUIQqgINUGhvcmt1cy5Qcm90b2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Phorkus.Proto.DefaultReflection.Descriptor, global::Phorkus.Proto.TransactionReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.Validator), global::Phorkus.Proto.Validator.Parser, new[]{ "Version", "PrevHash", "BlockIndex", "ValidatorIndex", "Timestamp", "ViewNumber" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.ChangeViewRequest), global::Phorkus.Proto.ChangeViewRequest.Parser, new[]{ "Validator", "NewViewNumber" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.BlockPrepareRequest), global::Phorkus.Proto.BlockPrepareRequest.Parser, new[]{ "Validator", "Nonce", "TransactionHashes", "MinerTransaction", "Timestamp", "Signature" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Proto.BlockPrepareReply), global::Phorkus.Proto.BlockPrepareReply.Parser, new[]{ "Validator", "Signature" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class Validator : pb::IMessage<Validator> {
    private static readonly pb::MessageParser<Validator> _parser = new pb::MessageParser<Validator>(() => new Validator());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Validator> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ConsensusReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Validator() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Validator(Validator other) : this() {
      version_ = other.version_;
      prevHash_ = other.prevHash_ != null ? other.prevHash_.Clone() : null;
      blockIndex_ = other.blockIndex_;
      validatorIndex_ = other.validatorIndex_;
      timestamp_ = other.timestamp_;
      viewNumber_ = other.viewNumber_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Validator Clone() {
      return new Validator(this);
    }

    /// <summary>Field number for the "version" field.</summary>
    public const int VersionFieldNumber = 1;
    private uint version_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Version {
      get { return version_; }
      set {
        version_ = value;
      }
    }

    /// <summary>Field number for the "prev_hash" field.</summary>
    public const int PrevHashFieldNumber = 2;
    private global::Phorkus.Proto.UInt256 prevHash_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.UInt256 PrevHash {
      get { return prevHash_; }
      set {
        prevHash_ = value;
      }
    }

    /// <summary>Field number for the "block_index" field.</summary>
    public const int BlockIndexFieldNumber = 3;
    private ulong blockIndex_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong BlockIndex {
      get { return blockIndex_; }
      set {
        blockIndex_ = value;
      }
    }

    /// <summary>Field number for the "validator_index" field.</summary>
    public const int ValidatorIndexFieldNumber = 4;
    private uint validatorIndex_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ValidatorIndex {
      get { return validatorIndex_; }
      set {
        validatorIndex_ = value;
      }
    }

    /// <summary>Field number for the "timestamp" field.</summary>
    public const int TimestampFieldNumber = 5;
    private ulong timestamp_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Timestamp {
      get { return timestamp_; }
      set {
        timestamp_ = value;
      }
    }

    /// <summary>Field number for the "view_number" field.</summary>
    public const int ViewNumberFieldNumber = 6;
    private uint viewNumber_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint ViewNumber {
      get { return viewNumber_; }
      set {
        viewNumber_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Validator);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Validator other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Version != other.Version) return false;
      if (!object.Equals(PrevHash, other.PrevHash)) return false;
      if (BlockIndex != other.BlockIndex) return false;
      if (ValidatorIndex != other.ValidatorIndex) return false;
      if (Timestamp != other.Timestamp) return false;
      if (ViewNumber != other.ViewNumber) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Version != 0) hash ^= Version.GetHashCode();
      if (prevHash_ != null) hash ^= PrevHash.GetHashCode();
      if (BlockIndex != 0UL) hash ^= BlockIndex.GetHashCode();
      if (ValidatorIndex != 0) hash ^= ValidatorIndex.GetHashCode();
      if (Timestamp != 0UL) hash ^= Timestamp.GetHashCode();
      if (ViewNumber != 0) hash ^= ViewNumber.GetHashCode();
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
      if (Version != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Version);
      }
      if (prevHash_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(PrevHash);
      }
      if (BlockIndex != 0UL) {
        output.WriteRawTag(24);
        output.WriteUInt64(BlockIndex);
      }
      if (ValidatorIndex != 0) {
        output.WriteRawTag(32);
        output.WriteUInt32(ValidatorIndex);
      }
      if (Timestamp != 0UL) {
        output.WriteRawTag(40);
        output.WriteUInt64(Timestamp);
      }
      if (ViewNumber != 0) {
        output.WriteRawTag(48);
        output.WriteUInt32(ViewNumber);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Version != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Version);
      }
      if (prevHash_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(PrevHash);
      }
      if (BlockIndex != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(BlockIndex);
      }
      if (ValidatorIndex != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ValidatorIndex);
      }
      if (Timestamp != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Timestamp);
      }
      if (ViewNumber != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(ViewNumber);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Validator other) {
      if (other == null) {
        return;
      }
      if (other.Version != 0) {
        Version = other.Version;
      }
      if (other.prevHash_ != null) {
        if (prevHash_ == null) {
          prevHash_ = new global::Phorkus.Proto.UInt256();
        }
        PrevHash.MergeFrom(other.PrevHash);
      }
      if (other.BlockIndex != 0UL) {
        BlockIndex = other.BlockIndex;
      }
      if (other.ValidatorIndex != 0) {
        ValidatorIndex = other.ValidatorIndex;
      }
      if (other.Timestamp != 0UL) {
        Timestamp = other.Timestamp;
      }
      if (other.ViewNumber != 0) {
        ViewNumber = other.ViewNumber;
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
            Version = input.ReadUInt32();
            break;
          }
          case 18: {
            if (prevHash_ == null) {
              prevHash_ = new global::Phorkus.Proto.UInt256();
            }
            input.ReadMessage(prevHash_);
            break;
          }
          case 24: {
            BlockIndex = input.ReadUInt64();
            break;
          }
          case 32: {
            ValidatorIndex = input.ReadUInt32();
            break;
          }
          case 40: {
            Timestamp = input.ReadUInt64();
            break;
          }
          case 48: {
            ViewNumber = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class ChangeViewRequest : pb::IMessage<ChangeViewRequest> {
    private static readonly pb::MessageParser<ChangeViewRequest> _parser = new pb::MessageParser<ChangeViewRequest>(() => new ChangeViewRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<ChangeViewRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ConsensusReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ChangeViewRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ChangeViewRequest(ChangeViewRequest other) : this() {
      validator_ = other.validator_ != null ? other.validator_.Clone() : null;
      newViewNumber_ = other.newViewNumber_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ChangeViewRequest Clone() {
      return new ChangeViewRequest(this);
    }

    /// <summary>Field number for the "validator" field.</summary>
    public const int ValidatorFieldNumber = 1;
    private global::Phorkus.Proto.Validator validator_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.Validator Validator {
      get { return validator_; }
      set {
        validator_ = value;
      }
    }

    /// <summary>Field number for the "new_view_number" field.</summary>
    public const int NewViewNumberFieldNumber = 2;
    private uint newViewNumber_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint NewViewNumber {
      get { return newViewNumber_; }
      set {
        newViewNumber_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as ChangeViewRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(ChangeViewRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Validator, other.Validator)) return false;
      if (NewViewNumber != other.NewViewNumber) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (validator_ != null) hash ^= Validator.GetHashCode();
      if (NewViewNumber != 0) hash ^= NewViewNumber.GetHashCode();
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
      if (validator_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Validator);
      }
      if (NewViewNumber != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(NewViewNumber);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (validator_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Validator);
      }
      if (NewViewNumber != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(NewViewNumber);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(ChangeViewRequest other) {
      if (other == null) {
        return;
      }
      if (other.validator_ != null) {
        if (validator_ == null) {
          validator_ = new global::Phorkus.Proto.Validator();
        }
        Validator.MergeFrom(other.Validator);
      }
      if (other.NewViewNumber != 0) {
        NewViewNumber = other.NewViewNumber;
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
            if (validator_ == null) {
              validator_ = new global::Phorkus.Proto.Validator();
            }
            input.ReadMessage(validator_);
            break;
          }
          case 16: {
            NewViewNumber = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class BlockPrepareRequest : pb::IMessage<BlockPrepareRequest> {
    private static readonly pb::MessageParser<BlockPrepareRequest> _parser = new pb::MessageParser<BlockPrepareRequest>(() => new BlockPrepareRequest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BlockPrepareRequest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ConsensusReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BlockPrepareRequest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BlockPrepareRequest(BlockPrepareRequest other) : this() {
      validator_ = other.validator_ != null ? other.validator_.Clone() : null;
      nonce_ = other.nonce_;
      transactionHashes_ = other.transactionHashes_.Clone();
      minerTransaction_ = other.minerTransaction_ != null ? other.minerTransaction_.Clone() : null;
      timestamp_ = other.timestamp_;
      signature_ = other.signature_ != null ? other.signature_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BlockPrepareRequest Clone() {
      return new BlockPrepareRequest(this);
    }

    /// <summary>Field number for the "validator" field.</summary>
    public const int ValidatorFieldNumber = 1;
    private global::Phorkus.Proto.Validator validator_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.Validator Validator {
      get { return validator_; }
      set {
        validator_ = value;
      }
    }

    /// <summary>Field number for the "nonce" field.</summary>
    public const int NonceFieldNumber = 2;
    private ulong nonce_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Nonce {
      get { return nonce_; }
      set {
        nonce_ = value;
      }
    }

    /// <summary>Field number for the "transaction_hashes" field.</summary>
    public const int TransactionHashesFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Phorkus.Proto.UInt256> _repeated_transactionHashes_codec
        = pb::FieldCodec.ForMessage(26, global::Phorkus.Proto.UInt256.Parser);
    private readonly pbc::RepeatedField<global::Phorkus.Proto.UInt256> transactionHashes_ = new pbc::RepeatedField<global::Phorkus.Proto.UInt256>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Phorkus.Proto.UInt256> TransactionHashes {
      get { return transactionHashes_; }
    }

    /// <summary>Field number for the "miner_transaction" field.</summary>
    public const int MinerTransactionFieldNumber = 4;
    private global::Phorkus.Proto.Transaction minerTransaction_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.Transaction MinerTransaction {
      get { return minerTransaction_; }
      set {
        minerTransaction_ = value;
      }
    }

    /// <summary>Field number for the "timestamp" field.</summary>
    public const int TimestampFieldNumber = 5;
    private ulong timestamp_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Timestamp {
      get { return timestamp_; }
      set {
        timestamp_ = value;
      }
    }

    /// <summary>Field number for the "signature" field.</summary>
    public const int SignatureFieldNumber = 6;
    private global::Phorkus.Proto.Signature signature_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.Signature Signature {
      get { return signature_; }
      set {
        signature_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BlockPrepareRequest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BlockPrepareRequest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Validator, other.Validator)) return false;
      if (Nonce != other.Nonce) return false;
      if(!transactionHashes_.Equals(other.transactionHashes_)) return false;
      if (!object.Equals(MinerTransaction, other.MinerTransaction)) return false;
      if (Timestamp != other.Timestamp) return false;
      if (!object.Equals(Signature, other.Signature)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (validator_ != null) hash ^= Validator.GetHashCode();
      if (Nonce != 0UL) hash ^= Nonce.GetHashCode();
      hash ^= transactionHashes_.GetHashCode();
      if (minerTransaction_ != null) hash ^= MinerTransaction.GetHashCode();
      if (Timestamp != 0UL) hash ^= Timestamp.GetHashCode();
      if (signature_ != null) hash ^= Signature.GetHashCode();
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
      if (validator_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Validator);
      }
      if (Nonce != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(Nonce);
      }
      transactionHashes_.WriteTo(output, _repeated_transactionHashes_codec);
      if (minerTransaction_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(MinerTransaction);
      }
      if (Timestamp != 0UL) {
        output.WriteRawTag(40);
        output.WriteUInt64(Timestamp);
      }
      if (signature_ != null) {
        output.WriteRawTag(50);
        output.WriteMessage(Signature);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (validator_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Validator);
      }
      if (Nonce != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Nonce);
      }
      size += transactionHashes_.CalculateSize(_repeated_transactionHashes_codec);
      if (minerTransaction_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(MinerTransaction);
      }
      if (Timestamp != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Timestamp);
      }
      if (signature_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Signature);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BlockPrepareRequest other) {
      if (other == null) {
        return;
      }
      if (other.validator_ != null) {
        if (validator_ == null) {
          validator_ = new global::Phorkus.Proto.Validator();
        }
        Validator.MergeFrom(other.Validator);
      }
      if (other.Nonce != 0UL) {
        Nonce = other.Nonce;
      }
      transactionHashes_.Add(other.transactionHashes_);
      if (other.minerTransaction_ != null) {
        if (minerTransaction_ == null) {
          minerTransaction_ = new global::Phorkus.Proto.Transaction();
        }
        MinerTransaction.MergeFrom(other.MinerTransaction);
      }
      if (other.Timestamp != 0UL) {
        Timestamp = other.Timestamp;
      }
      if (other.signature_ != null) {
        if (signature_ == null) {
          signature_ = new global::Phorkus.Proto.Signature();
        }
        Signature.MergeFrom(other.Signature);
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
            if (validator_ == null) {
              validator_ = new global::Phorkus.Proto.Validator();
            }
            input.ReadMessage(validator_);
            break;
          }
          case 16: {
            Nonce = input.ReadUInt64();
            break;
          }
          case 26: {
            transactionHashes_.AddEntriesFrom(input, _repeated_transactionHashes_codec);
            break;
          }
          case 34: {
            if (minerTransaction_ == null) {
              minerTransaction_ = new global::Phorkus.Proto.Transaction();
            }
            input.ReadMessage(minerTransaction_);
            break;
          }
          case 40: {
            Timestamp = input.ReadUInt64();
            break;
          }
          case 50: {
            if (signature_ == null) {
              signature_ = new global::Phorkus.Proto.Signature();
            }
            input.ReadMessage(signature_);
            break;
          }
        }
      }
    }

  }

  public sealed partial class BlockPrepareReply : pb::IMessage<BlockPrepareReply> {
    private static readonly pb::MessageParser<BlockPrepareReply> _parser = new pb::MessageParser<BlockPrepareReply>(() => new BlockPrepareReply());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<BlockPrepareReply> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Proto.ConsensusReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BlockPrepareReply() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BlockPrepareReply(BlockPrepareReply other) : this() {
      validator_ = other.validator_ != null ? other.validator_.Clone() : null;
      signature_ = other.signature_ != null ? other.signature_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public BlockPrepareReply Clone() {
      return new BlockPrepareReply(this);
    }

    /// <summary>Field number for the "validator" field.</summary>
    public const int ValidatorFieldNumber = 1;
    private global::Phorkus.Proto.Validator validator_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.Validator Validator {
      get { return validator_; }
      set {
        validator_ = value;
      }
    }

    /// <summary>Field number for the "signature" field.</summary>
    public const int SignatureFieldNumber = 2;
    private global::Phorkus.Proto.Signature signature_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Proto.Signature Signature {
      get { return signature_; }
      set {
        signature_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as BlockPrepareReply);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(BlockPrepareReply other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Validator, other.Validator)) return false;
      if (!object.Equals(Signature, other.Signature)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (validator_ != null) hash ^= Validator.GetHashCode();
      if (signature_ != null) hash ^= Signature.GetHashCode();
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
      if (validator_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Validator);
      }
      if (signature_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Signature);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (validator_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Validator);
      }
      if (signature_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Signature);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(BlockPrepareReply other) {
      if (other == null) {
        return;
      }
      if (other.validator_ != null) {
        if (validator_ == null) {
          validator_ = new global::Phorkus.Proto.Validator();
        }
        Validator.MergeFrom(other.Validator);
      }
      if (other.signature_ != null) {
        if (signature_ == null) {
          signature_ = new global::Phorkus.Proto.Signature();
        }
        Signature.MergeFrom(other.Signature);
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
            if (validator_ == null) {
              validator_ = new global::Phorkus.Proto.Validator();
            }
            input.ReadMessage(validator_);
            break;
          }
          case 18: {
            if (signature_ == null) {
              signature_ = new global::Phorkus.Proto.Signature();
            }
            input.ReadMessage(signature_);
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
