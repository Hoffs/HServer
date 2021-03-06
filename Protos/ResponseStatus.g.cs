// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Networking/ResponseStatus.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace HServer.Networking {

  /// <summary>Holder for reflection information generated from Networking/ResponseStatus.proto</summary>
  public static partial class ResponseStatusReflection {

    #region Descriptor
    /// <summary>File descriptor for Networking/ResponseStatus.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ResponseStatusReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Ch9OZXR3b3JraW5nL1Jlc3BvbnNlU3RhdHVzLnByb3RvEhJIU2VydmVyLk5l",
            "dHdvcmtpbmcqdgoOUmVzcG9uc2VTdGF0dXMSCwoHU1VDQ0VTUxAAEgkKBUVS",
            "Uk9SEAESDQoJRk9SQklEREVOEAISCwoHQ1JFQVRFRBADEg8KC0JBRF9SRVFV",
            "RVNUEAQSDQoJTk9UX0ZPVU5EEAUSEAoMVU5BVVRIT1JJWkVEEAZiBnByb3Rv",
            "Mw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::HServer.Networking.ResponseStatus), }, null));
    }
    #endregion

  }
  #region Enums
  public enum ResponseStatus {
    [pbr::OriginalName("SUCCESS")] Success = 0,
    [pbr::OriginalName("ERROR")] Error = 1,
    [pbr::OriginalName("FORBIDDEN")] Forbidden = 2,
    [pbr::OriginalName("CREATED")] Created = 3,
    [pbr::OriginalName("BAD_REQUEST")] BadRequest = 4,
    [pbr::OriginalName("NOT_FOUND")] NotFound = 5,
    [pbr::OriginalName("UNAUTHORIZED")] Unauthorized = 6,
  }

  #endregion

}

#endregion Designer generated code
