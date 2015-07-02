// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefAttributeTag
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  public enum TnefAttributeTag
  {
    Null = 0,
    From = 32768,
    Subject = 98308,
    MessageId = 98313,
    ParentId = 98314,
    ConversationId = 98315,
    AttachTitle = 98320,
    Body = 163852,
    DateStart = 196614,
    DateEnd = 196615,
    DateSent = 229381,
    DateReceived = 229382,
    AttachCreateDate = 229394,
    AttachModifyDate = 229395,
    DateModified = 229408,
    RequestResponse = 262153,
    Priority = 294925,
    AidOwner = 327688,
    Owner = 393216,
    SentFor = 393217,
    Delegate = 393218,
    MessageStatus = 425991,
    AttachData = 425999,
    AttachMetaFile = 426001,
    AttachTransportFilename = 430081,
    AttachRenderData = 430082,
    MapiProperties = 430083,
    RecipientTable = 430084,
    Attachment = 430085,
    OemCodepage = 430087,
    OriginalMessageClass = 458758,
    MessageClass = 491528,
    TnefVersion = 561158,
  }
}
