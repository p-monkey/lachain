// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: account.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Phorkus.Core.Proto {

  /// <summary>Holder for reflection information generated from account.proto</summary>
  public static partial class AccountReflection {

    #region Descriptor
    /// <summary>File descriptor for account.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static AccountReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Cg1hY2NvdW50LnByb3RvGg1kZWZhdWx0LnByb3RvIkIKB0FjY291bnQSGQoH",
            "YWRkcmVzcxgBIAEoCzIILlVJbnQxNjASHAoFc3RhdGUYAiABKA4yDS5BY2Nv",
            "dW50U3RhdGUiFgoFVGVzdDESDQoFdmFsdWUYASABKA0iJQoFVGVzdDISDQoF",
            "dmFsdWUYASABKA0SDQoFaGVsbG8YAiABKA0qQgoMQWNjb3VudFN0YXRlEhgK",
            "FEFDQ09VTlRfU1RBVEVfQUNUSVZFEAASGAoUQUNDT1VOVF9TVEFURV9GUk9a",
            "RU4QAUIVqgISUGhvcmt1cy5Db3JlLlByb3RvYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Phorkus.Core.Proto.DefaultReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Phorkus.Core.Proto.AccountState), }, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Core.Proto.Account), global::Phorkus.Core.Proto.Account.Parser, new[]{ "Address", "State" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Core.Proto.Test1), global::Phorkus.Core.Proto.Test1.Parser, new[]{ "Value" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Phorkus.Core.Proto.Test2), global::Phorkus.Core.Proto.Test2.Parser, new[]{ "Value", "Hello" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum AccountState {
    [pbr::OriginalName("ACCOUNT_STATE_ACTIVE")] Active = 0,
    [pbr::OriginalName("ACCOUNT_STATE_FROZEN")] Frozen = 1,
  }

  #endregion

  #region Messages
  public sealed partial class Account : pb::IMessage<Account> {
    private static readonly pb::MessageParser<Account> _parser = new pb::MessageParser<Account>(() => new Account());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Account> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Core.Proto.AccountReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Account() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Account(Account other) : this() {
      address_ = other.address_ != null ? other.address_.Clone() : null;
      state_ = other.state_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Account Clone() {
      return new Account(this);
    }

    /// <summary>Field number for the "address" field.</summary>
    public const int AddressFieldNumber = 1;
    private global::Phorkus.Core.Proto.UInt160 address_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Core.Proto.UInt160 Address {
      get { return address_; }
      set {
        address_ = value;
      }
    }

    /// <summary>Field number for the "state" field.</summary>
    public const int StateFieldNumber = 2;
    private global::Phorkus.Core.Proto.AccountState state_ = 0;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Phorkus.Core.Proto.AccountState State {
      get { return state_; }
      set {
        state_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Account);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Account other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Address, other.Address)) return false;
      if (State != other.State) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (address_ != null) hash ^= Address.GetHashCode();
      if (State != 0) hash ^= State.GetHashCode();
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
      if (address_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Address);
      }
      if (State != 0) {
        output.WriteRawTag(16);
        output.WriteEnum((int) State);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (address_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Address);
      }
      if (State != 0) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) State);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Account other) {
      if (other == null) {
        return;
      }
      if (other.address_ != null) {
        if (address_ == null) {
          address_ = new global::Phorkus.Core.Proto.UInt160();
        }
        Address.MergeFrom(other.Address);
      }
      if (other.State != 0) {
        State = other.State;
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
            if (address_ == null) {
              address_ = new global::Phorkus.Core.Proto.UInt160();
            }
            input.ReadMessage(address_);
            break;
          }
          case 16: {
            state_ = (global::Phorkus.Core.Proto.AccountState) input.ReadEnum();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Test1 : pb::IMessage<Test1> {
    private static readonly pb::MessageParser<Test1> _parser = new pb::MessageParser<Test1>(() => new Test1());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Test1> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Core.Proto.AccountReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Test1() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Test1(Test1 other) : this() {
      value_ = other.value_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Test1 Clone() {
      return new Test1(this);
    }

    /// <summary>Field number for the "value" field.</summary>
    public const int ValueFieldNumber = 1;
    private uint value_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Value {
      get { return value_; }
      set {
        value_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Test1);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Test1 other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Value != other.Value) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Value != 0) hash ^= Value.GetHashCode();
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
      if (Value != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Value);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Value != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Value);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Test1 other) {
      if (other == null) {
        return;
      }
      if (other.Value != 0) {
        Value = other.Value;
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
            Value = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  public sealed partial class Test2 : pb::IMessage<Test2> {
    private static readonly pb::MessageParser<Test2> _parser = new pb::MessageParser<Test2>(() => new Test2());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Test2> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Phorkus.Core.Proto.AccountReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Test2() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Test2(Test2 other) : this() {
      value_ = other.value_;
      hello_ = other.hello_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Test2 Clone() {
      return new Test2(this);
    }

    /// <summary>Field number for the "value" field.</summary>
    public const int ValueFieldNumber = 1;
    private uint value_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Value {
      get { return value_; }
      set {
        value_ = value;
      }
    }

    /// <summary>Field number for the "hello" field.</summary>
    public const int HelloFieldNumber = 2;
    private uint hello_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Hello {
      get { return hello_; }
      set {
        hello_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Test2);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Test2 other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Value != other.Value) return false;
      if (Hello != other.Hello) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Value != 0) hash ^= Value.GetHashCode();
      if (Hello != 0) hash ^= Hello.GetHashCode();
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
      if (Value != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Value);
      }
      if (Hello != 0) {
        output.WriteRawTag(16);
        output.WriteUInt32(Hello);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Value != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Value);
      }
      if (Hello != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Hello);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Test2 other) {
      if (other == null) {
        return;
      }
      if (other.Value != 0) {
        Value = other.Value;
      }
      if (other.Hello != 0) {
        Hello = other.Hello;
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
            Value = input.ReadUInt32();
            break;
          }
          case 16: {
            Hello = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
