// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: service/gate/survey_gate.proto
#region Designer generated code

using System; 
using System.Threading.Tasks; 
using DotBPE.Rpc; 
using DotBPE.Protocol.Amp; 
using DotBPE.Rpc.Exceptions; 
using Google.Protobuf; 

namespace Survey.Core {

//start for class SurveyGateServiceClient
public sealed class SurveyGateServiceClient : AmpInvokeClient 
{
public SurveyGateServiceClient(IRpcClient<AmpMessage> client) : base(client)
{
}
public SurveyGateServiceClient(string remoteAddress) : base(remoteAddress)
{
}
public async Task<SaveAPaperRsp> SaveAPaperAsync(SaveAPaperReq request,int timeOut=3000)
{
AmpMessage message = AmpMessage.CreateRequestMessage(10000, 1);
message.Data = request.ToByteArray();
var response = await base.CallInvoker.AsyncCall(message,timeOut);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new SaveAPaperRsp();
}
return SaveAPaperRsp.Parser.ParseFrom(response.Data);
}

//同步方法
public SaveAPaperRsp SaveAPaper(SaveAPaperReq request)
{
AmpMessage message = AmpMessage.CreateRequestMessage(10000, 1);
message.Data = request.ToByteArray();
var response =  base.CallInvoker.BlockingCall(message);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new SaveAPaperRsp();
}
return SaveAPaperRsp.Parser.ParseFrom(response.Data);
}
public async Task<APaperListRsp> QueryAPaperListAsync(QueryAPaperReq request,int timeOut=3000)
{
AmpMessage message = AmpMessage.CreateRequestMessage(10000, 2);
message.Data = request.ToByteArray();
var response = await base.CallInvoker.AsyncCall(message,timeOut);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new APaperListRsp();
}
return APaperListRsp.Parser.ParseFrom(response.Data);
}

//同步方法
public APaperListRsp QueryAPaperList(QueryAPaperReq request)
{
AmpMessage message = AmpMessage.CreateRequestMessage(10000, 2);
message.Data = request.ToByteArray();
var response =  base.CallInvoker.BlockingCall(message);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new APaperListRsp();
}
return APaperListRsp.Parser.ParseFrom(response.Data);
}
public async Task<APaperRsp> GetAPaperAsync(GetAPaperReq request,int timeOut=3000)
{
AmpMessage message = AmpMessage.CreateRequestMessage(10000, 3);
message.Data = request.ToByteArray();
var response = await base.CallInvoker.AsyncCall(message,timeOut);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new APaperRsp();
}
return APaperRsp.Parser.ParseFrom(response.Data);
}

//同步方法
public APaperRsp GetAPaper(GetAPaperReq request)
{
AmpMessage message = AmpMessage.CreateRequestMessage(10000, 3);
message.Data = request.ToByteArray();
var response =  base.CallInvoker.BlockingCall(message);
if (response == null)
{
throw new RpcException("error,response is null !");
}
if (response.Data == null)
{
return new APaperRsp();
}
return APaperRsp.Parser.ParseFrom(response.Data);
}
}
//end for class SurveyGateServiceClient
}
#endregion
