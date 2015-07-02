using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal static class MimeData {

        static MimeData() {
            var headerNameIndexArray = new HeaderNameIndex[121];
            headerNameIndexArray[0] = HeaderNameIndex.ReturnPath;
            headerNameIndexArray[3] = HeaderNameIndex.MessageId;
            headerNameIndexArray[4] = HeaderNameIndex.Subject;
            headerNameIndexArray[5] = (HeaderNameIndex) 5;
            headerNameIndexArray[7] = HeaderNameIndex.Summary;
            headerNameIndexArray[10] = HeaderNameIndex.XPriority;
            headerNameIndexArray[11] = HeaderNameIndex.Precedence;
            headerNameIndexArray[12] = HeaderNameIndex.ListUnsubscribe;
            headerNameIndexArray[18] = HeaderNameIndex.RR;
            headerNameIndexArray[20] = HeaderNameIndex.Comments;
            headerNameIndexArray[24] = HeaderNameIndex.ContentLocation;
            headerNameIndexArray[26] = HeaderNameIndex.ResentFrom;
            headerNameIndexArray[27] = HeaderNameIndex.ContentLanguage;
            headerNameIndexArray[28] = HeaderNameIndex.NewsGroups;
            headerNameIndexArray[29] = HeaderNameIndex.ResentCc;
            headerNameIndexArray[30] = HeaderNameIndex.ResentBcc;
            headerNameIndexArray[34] = HeaderNameIndex.ResentSender;
            headerNameIndexArray[36] = HeaderNameIndex.ListHelp;
            headerNameIndexArray[37] = HeaderNameIndex.From;
            headerNameIndexArray[38] = HeaderNameIndex.ReturnReceiptTo;
            headerNameIndexArray[40] = HeaderNameIndex.Lines;
            headerNameIndexArray[42] = HeaderNameIndex.ContentDisposition;
            headerNameIndexArray[43] = HeaderNameIndex.Bytes;
            headerNameIndexArray[48] = HeaderNameIndex.ContentId;
            headerNameIndexArray[49] = HeaderNameIndex.FollowUpTo;
            headerNameIndexArray[52] = HeaderNameIndex.ReplyBy;
            headerNameIndexArray[55] = HeaderNameIndex.Bcc;
            headerNameIndexArray[59] = HeaderNameIndex.ResentMessageId;
            headerNameIndexArray[60] = (HeaderNameIndex) 38;
            headerNameIndexArray[61] = HeaderNameIndex.XMSMailPriority;
            headerNameIndexArray[64] = HeaderNameIndex.XExchangeBcc;
            headerNameIndexArray[66] = HeaderNameIndex.DeferredDelivery;
            headerNameIndexArray[67] = HeaderNameIndex.Expires;
            headerNameIndexArray[70] = (HeaderNameIndex) 43;
            headerNameIndexArray[71] = HeaderNameIndex.Organization;
            headerNameIndexArray[72] = HeaderNameIndex.ContentTransferEncoding;
            headerNameIndexArray[78] = HeaderNameIndex.Encoding;
            headerNameIndexArray[80] = HeaderNameIndex.XExchangeCrossPremisesBcc;
            headerNameIndexArray[81] = HeaderNameIndex.ContentType;
            headerNameIndexArray[82] = HeaderNameIndex.ApparentlyTo;
            headerNameIndexArray[83] = HeaderNameIndex.Approved;
            headerNameIndexArray[84] = (HeaderNameIndex) 51;
            headerNameIndexArray[85] = HeaderNameIndex.Path;
            headerNameIndexArray[86] = HeaderNameIndex.References;
            headerNameIndexArray[88] = HeaderNameIndex.ResentTo;
            headerNameIndexArray[89] = HeaderNameIndex.ResentReplyTo;
            headerNameIndexArray[91] = HeaderNameIndex.Control;
            headerNameIndexArray[94] = HeaderNameIndex.InReplyTo;
            headerNameIndexArray[96] = HeaderNameIndex.ListSubscribe;
            headerNameIndexArray[97] = HeaderNameIndex.ContentDescription;
            headerNameIndexArray[101] = HeaderNameIndex.Encrypted;
            headerNameIndexArray[102] = (HeaderNameIndex) 66;
            headerNameIndexArray[103] = HeaderNameIndex.MimeVersion;
            headerNameIndexArray[106] = HeaderNameIndex.DispositionNotificationTo;
            headerNameIndexArray[111] = HeaderNameIndex.Distribution;
            headerNameIndexArray[117] = HeaderNameIndex.XRef;
            headerNameIndexArray[118] = HeaderNameIndex.Sensitivity;
            headerNameIndexArray[119] = HeaderNameIndex.ExpiryDate;
            headerNameIndexArray[120] = HeaderNameIndex.Priority;
            nameHashTable = headerNameIndexArray;
            nameIndex = new HeaderNameIndex[71] {
                HeaderNameIndex.Unknown,
                HeaderNameIndex.Received,
                HeaderNameIndex.Date,
                HeaderNameIndex.From,
                HeaderNameIndex.Subject,
                HeaderNameIndex.Sender,
                HeaderNameIndex.To,
                HeaderNameIndex.Cc,
                HeaderNameIndex.Bcc,
                HeaderNameIndex.MessageId,
                HeaderNameIndex.InReplyTo,
                HeaderNameIndex.References,
                HeaderNameIndex.ReturnPath,
                HeaderNameIndex.Comments,
                HeaderNameIndex.Keywords,
                HeaderNameIndex.Encrypted,
                HeaderNameIndex.ReplyBy,
                HeaderNameIndex.ReplyTo,
                HeaderNameIndex.ResentDate,
                HeaderNameIndex.ResentSender,
                HeaderNameIndex.ResentFrom,
                HeaderNameIndex.ResentBcc,
                HeaderNameIndex.ResentCc,
                HeaderNameIndex.ResentTo,
                HeaderNameIndex.ResentReplyTo,
                HeaderNameIndex.ResentMessageId,
                HeaderNameIndex.ApparentlyTo,
                HeaderNameIndex.Approved,
                HeaderNameIndex.Control,
                HeaderNameIndex.DeferredDelivery,
                HeaderNameIndex.Distribution,
                HeaderNameIndex.Encoding,
                HeaderNameIndex.Expires,
                HeaderNameIndex.ExpiryDate,
                HeaderNameIndex.FollowUpTo,
                HeaderNameIndex.Lines,
                HeaderNameIndex.Bytes,
                HeaderNameIndex.Article,
                HeaderNameIndex.Importance,
                HeaderNameIndex.Supercedes,
                HeaderNameIndex.NewsGroups,
                HeaderNameIndex.NntpPostingHost,
                HeaderNameIndex.Organization,
                HeaderNameIndex.Path,
                HeaderNameIndex.Precedence,
                HeaderNameIndex.Priority,
                HeaderNameIndex.ReturnReceiptTo,
                HeaderNameIndex.DispositionNotificationTo,
                HeaderNameIndex.RR,
                HeaderNameIndex.Sensitivity,
                HeaderNameIndex.Summary,
                HeaderNameIndex.XRef,
                HeaderNameIndex.AdHoc,
                HeaderNameIndex.ListHelp,
                HeaderNameIndex.ListSubscribe,
                HeaderNameIndex.ListUnsubscribe,
                HeaderNameIndex.MimeVersion,
                HeaderNameIndex.ContentBase,
                HeaderNameIndex.ContentClass,
                HeaderNameIndex.ContentDescription,
                HeaderNameIndex.ContentDisposition,
                HeaderNameIndex.ContentId,
                HeaderNameIndex.ContentLanguage,
                HeaderNameIndex.ContentLocation,
                HeaderNameIndex.ContentMD5,
                HeaderNameIndex.ContentTransferEncoding,
                HeaderNameIndex.ContentType,
                HeaderNameIndex.XPriority,
                HeaderNameIndex.XMSMailPriority,
                HeaderNameIndex.XExchangeBcc,
                HeaderNameIndex.XExchangeCrossPremisesBcc
            };
            valueHashTable = new int[201] {
                1,
                0,
                2,
                3,
                4,
                0,
                0,
                0,
                0,
                5,
                0,
                7,
                0,
                0,
                0,
                9,
                0,
                0,
                11,
                12,
                0,
                0,
                0,
                0,
                0,
                0,
                13,
                15,
                0,
                16,
                0,
                17,
                18,
                19,
                0,
                22,
                23,
                0,
                25,
                27,
                28,
                29,
                31,
                32,
                0,
                33,
                0,
                34,
                35,
                0,
                0,
                38,
                39,
                40,
                0,
                41,
                42,
                43,
                0,
                0,
                0,
                44,
                0,
                45,
                46,
                47,
                48,
                0,
                0,
                0,
                50,
                0,
                0,
                0,
                51,
                52,
                54,
                58,
                0,
                61,
                63,
                0,
                64,
                0,
                0,
                0,
                65,
                66,
                67,
                68,
                0,
                69,
                0,
                0,
                0,
                0,
                70,
                0,
                0,
                0,
                72,
                0,
                0,
                0,
                73,
                0,
                0,
                74,
                75,
                76,
                77,
                0,
                79,
                80,
                82,
                0,
                0,
                0,
                83,
                85,
                88,
                0,
                90,
                91,
                93,
                0,
                0,
                0,
                94,
                0,
                0,
                0,
                95,
                0,
                97,
                99,
                0,
                100,
                101,
                0,
                102,
                0,
                0,
                0,
                103,
                105,
                106,
                107,
                0,
                109,
                110,
                0,
                0,
                0,
                0,
                0,
                111,
                113,
                116,
                0,
                117,
                118,
                0,
                0,
                119,
                0,
                0,
                120,
                121,
                122,
                123,
                125,
                0,
                sbyte.MaxValue,
                0,
                128,
                0,
                0,
                129,
                0,
                131,
                0,
                0,
                0,
                132,
                133,
                0,
                134,
                135,
                0,
                136,
                137,
                0,
                139,
                140,
                0,
                142,
                0,
                0,
                145,
                146
            };
            values = new HeaderValueDef[148] {
                new HeaderValueDef(0, null),
                new HeaderValueDef(0, "text/enriched"),
                new HeaderValueDef(2, "total"),
                new HeaderValueDef(3, "inline"),
                new HeaderValueDef(4, "message/disposition-notification"),
                new HeaderValueDef(9, "mixed"),
                new HeaderValueDef(9, "basic"),
                new HeaderValueDef(11, "xml"),
                new HeaderValueDef(11, "id"),
                new HeaderValueDef(15, "uuencode"),
                new HeaderValueDef(15, "msaccess"),
                new HeaderValueDef(18, "number"),
                new HeaderValueDef(19, "application/ms-tnef"),
                new HeaderValueDef(26, "video/avi"),
                new HeaderValueDef(26, "gif"),
                new HeaderValueDef(27, "binary"),
                new HeaderValueDef(29, "ms-publisher"),
                new HeaderValueDef(31, "multipart/digest"),
                new HeaderValueDef(32, "disposition-notification"),
                new HeaderValueDef(33, "enriched"),
                new HeaderValueDef(33, "boundary"),
                new HeaderValueDef(33, "digest"),
                new HeaderValueDef(35, "application/x-pkcs7-signature"),
                new HeaderValueDef(36, "start-info"),
                new HeaderValueDef(36, "charset"),
                new HeaderValueDef(38, "avi"),
                new HeaderValueDef(38, "msword"),
                new HeaderValueDef(39, "ms-vsi"),
                new HeaderValueDef(40, "schdpl32"),
                new HeaderValueDef(41, "external-body"),
                new HeaderValueDef(41, "text/xml"),
                new HeaderValueDef(42, "x-pkcs7-mime"),
                new HeaderValueDef(43, "base64"),
                new HeaderValueDef(45, "access-type"),
                new HeaderValueDef(47, "format"),
                new HeaderValueDef(48, "ms-powerpoint"),
                new HeaderValueDef(48, "padding"),
                new HeaderValueDef(48, "application/pgp-signature"),
                new HeaderValueDef(51, "pgp-signature"),
                new HeaderValueDef(52, "server"),
                new HeaderValueDef(53, "report-type"),
                new HeaderValueDef(55, "bmp"),
                new HeaderValueDef(56, "text"),
                new HeaderValueDef(57, "application/octet-stream"),
                new HeaderValueDef(61, "read-date"),
                new HeaderValueDef(63, "multipart"),
                new HeaderValueDef(64, "mpg"),
                new HeaderValueDef(65, "multipart/parallel"),
                new HeaderValueDef(66, "html"),
                new HeaderValueDef(66, "encrypted"),
                new HeaderValueDef(70, "mode"),
                new HeaderValueDef(74, "octet-stream"),
                new HeaderValueDef(75, "x-pkcs7-signature"),
                new HeaderValueDef(75, "tiff"),
                new HeaderValueDef(76, "multipart/form-data"),
                new HeaderValueDef(76, "multipart/signed"),
                new HeaderValueDef(76, "image/bmp"),
                new HeaderValueDef(76, "permission"),
                new HeaderValueDef(77, "partial"),
                new HeaderValueDef(77, "richtext"),
                new HeaderValueDef(77, "pkcs7-signature"),
                new HeaderValueDef(79, "attachment"),
                new HeaderValueDef(79, "mac-binhex40"),
                new HeaderValueDef(80, "audio"),
                new HeaderValueDef(82, "version"),
                new HeaderValueDef(86, "report"),
                new HeaderValueDef(87, "parallel"),
                new HeaderValueDef(88, "multipart/alternative"),
                new HeaderValueDef(89, "creation-date"),
                new HeaderValueDef(91, "audio/mp3"),
                new HeaderValueDef(96, "image"),
                new HeaderValueDef(96, "application/x-pkcs7-mime"),
                new HeaderValueDef(100, "7bit"),
                new HeaderValueDef(104, "delivery-status"),
                new HeaderValueDef(107, "application/pkcs7-mime"),
                new HeaderValueDef(108, "mp3"),
                new HeaderValueDef(109, "mp4"),
                new HeaderValueDef(110, "text/richtext"),
                new HeaderValueDef(110, "modification-date"),
                new HeaderValueDef(112, "text/plain"),
                new HeaderValueDef(113, "pkcs7-mime"),
                new HeaderValueDef(113, "alternative"),
                new HeaderValueDef(114, "expiration"),
                new HeaderValueDef(118, "multipart/encrypted"),
                new HeaderValueDef(118, "css"),
                new HeaderValueDef(119, "image/png"),
                new HeaderValueDef(119, "form-data"),
                new HeaderValueDef(119, "message/delivery-status"),
                new HeaderValueDef(120, "png"),
                new HeaderValueDef(120, "smime-type"),
                new HeaderValueDef(122, "message/external-body"),
                new HeaderValueDef(123, "jpeg"),
                new HeaderValueDef(123, "video"),
                new HeaderValueDef(124, "postscript"),
                new HeaderValueDef(128, "application/mac-binhex40"),
                new HeaderValueDef(132, "text/calendar"),
                new HeaderValueDef(132, "image/gif"),
                new HeaderValueDef(134, "site"),
                new HeaderValueDef(134, "plain"),
                new HeaderValueDef(135, "message/rfc822"),
                new HeaderValueDef(137, "type"),
                new HeaderValueDef(138, "image/jpeg"),
                new HeaderValueDef(140, "msvideo"),
                new HeaderValueDef(144, "related"),
                new HeaderValueDef(144, "subject"),
                new HeaderValueDef(145, "pkcs10"),
                new HeaderValueDef(146, "pdf"),
                new HeaderValueDef(147, "midi"),
                new HeaderValueDef(147, "message"),
                new HeaderValueDef(149, "rfc822"),
                new HeaderValueDef(150, "size"),
                new HeaderValueDef(156, "audio/basic"),
                new HeaderValueDef(156, "text/html"),
                new HeaderValueDef(157, "ms-infopath.xml"),
                new HeaderValueDef(157, "multipart/related"),
                new HeaderValueDef(157, "mpeg4"),
                new HeaderValueDef(158, "ms-excel"),
                new HeaderValueDef(160, "application/postscript"),
                new HeaderValueDef(161, "ms-tnef"),
                new HeaderValueDef(164, "multipart/report"),
                new HeaderValueDef(167, "x-uue"),
                new HeaderValueDef(168, "directory"),
                new HeaderValueDef(169, "x-vcard"),
                new HeaderValueDef(170, "protocol"),
                new HeaderValueDef(170, "video/mpeg"),
                new HeaderValueDef(171, "delsp"),
                new HeaderValueDef(171, "filename"),
                new HeaderValueDef(173, "x-uuencode"),
                new HeaderValueDef(175, "name"),
                new HeaderValueDef(178, "signed"),
                new HeaderValueDef(178, "rfc822-headers"),
                new HeaderValueDef(180, "micalg"),
                new HeaderValueDef(184, "text/rfc822-headers"),
                new HeaderValueDef(185, "calendar"),
                new HeaderValueDef(187, "message/partial"),
                new HeaderValueDef(188, "application"),
                new HeaderValueDef(190, "mid"),
                new HeaderValueDef(191, "image/tiff"),
                new HeaderValueDef(191, "8bit"),
                new HeaderValueDef(193, "mpeg"),
                new HeaderValueDef(194, "application/pkcs7-signature"),
                new HeaderValueDef(194, "wav"),
                new HeaderValueDef(196, "quoted-printable"),
                new HeaderValueDef(196, "multipart/mixed"),
                new HeaderValueDef(196, "ms-mediapackage"),
                new HeaderValueDef(199, "pjpeg"),
                new HeaderValueDef(200, "start"),
                new HeaderValueDef(201)
            };
        }

        public static int HashName(byte[] chars, int off, int len) {
            var num = len;
            while (len != 0) {
                num = (num << 3) + (num >> 5) ^ (chars[off] | 32);
                --len;
                ++off;
            }
            return (num & int.MaxValue)%121;
        }

        public static int HashName(string chars) {
            var num = chars.Length;
            for (var index = 0; index != chars.Length; ++index) {
                num = (num << 3) + (num >> 5) ^ (chars[index] | 32);
            }
            return (num & int.MaxValue)%121;
        }

        public static int HashValue(string chars, int off, int len) {
            var num = 0;
            while (len != 0) {
                num = (num << 3) + (num >> 5) ^ (chars[off] | 32);
                --len;
                ++off;
            }
            return (num & int.MaxValue)%201;
        }

        public static int HashValue(byte[] chars, int off, int len) {
            var num = 0;
            while (len != 0) {
                num = (num << 3) + (num >> 5) ^ (chars[off] | 32);
                --len;
                ++off;
            }
            return (num & int.MaxValue)%201;
        }

        public static int HashValue(string chars) {
            var num = 0;
            for (var index = 0; index != chars.Length; ++index) {
                num = (num << 3) + (num >> 5) ^ (chars[index] | 32);
            }
            return (num & int.MaxValue)%201;
        }

        public static int HashValueAdd(int hash, string chars) {
            for (var index = 0; index != chars.Length; ++index) {
                hash = (hash << 3) + (hash >> 5) ^ (chars[index] | 32);
            }
            return hash;
        }

        public static int HashValueFinish(int hash) {
            return (hash & int.MaxValue)%201;
        }

        public const short MIN_HEADER_NAME = 2;
        public const short MAX_HEADER_NAME = 31;
        public const short MIN_VALUE = 2;
        public const short MAX_VALUE = 32;

        public static HeaderNameDef[] headerNames = new HeaderNameDef[80] {
            new HeaderNameDef(0, null, HeaderType.Text, HeaderId.Unknown, false),
            new HeaderNameDef(0, "Return-Path", HeaderType.AsciiText, HeaderId.ReturnPath, false),
            new HeaderNameDef(0, "Resent-Date", HeaderType.Date, HeaderId.ResentDate, false),
            new HeaderNameDef(3, "Message-ID", HeaderType.AsciiText, HeaderId.MessageId, false),
            new HeaderNameDef(4, "Subject", HeaderType.Text, HeaderId.Subject, false),
            new HeaderNameDef(5, "X-Mailer", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(7, "Summary", HeaderType.AsciiText, HeaderId.Summary, false),
            new HeaderNameDef(10, "X-Priority", HeaderType.AsciiText, HeaderId.XPriority, false),
            new HeaderNameDef(11, "Precedence", HeaderType.AsciiText, HeaderId.Precedence, false),
            new HeaderNameDef(12, "List-Unsubscribe", HeaderType.AsciiText, HeaderId.ListUnsubscribe, false),
            new HeaderNameDef(18, "Rr", HeaderType.AsciiText, HeaderId.RR, false),
            new HeaderNameDef(20, "Comments", HeaderType.Text, HeaderId.Comments, false),
            new HeaderNameDef(24, "Content-Location", HeaderType.AsciiText, HeaderId.ContentLocation, true),
            new HeaderNameDef(26, "Resent-From", HeaderType.Address, HeaderId.ResentFrom, false),
            new HeaderNameDef(27, "Content-Language", HeaderType.AsciiText, HeaderId.ContentLanguage, true),
            new HeaderNameDef(27, "Content-MD5", HeaderType.AsciiText, HeaderId.ContentMD5, true),
            new HeaderNameDef(28, "Newsgroups", HeaderType.AsciiText, HeaderId.NewsGroups, false),
            new HeaderNameDef(28, "Importance", HeaderType.AsciiText, HeaderId.Importance, false),
            new HeaderNameDef(29, "Resent-Cc", HeaderType.Address, HeaderId.ResentCc, false),
            new HeaderNameDef(30, "Resent-Bcc", HeaderType.Address, HeaderId.ResentBcc, false),
            new HeaderNameDef(34, "Resent-Sender", HeaderType.Address, HeaderId.ResentSender, false),
            new HeaderNameDef(34, "Content-Class", HeaderType.AsciiText, HeaderId.ContentClass, true),
            new HeaderNameDef(36, "List-Help", HeaderType.AsciiText, HeaderId.ListHelp, false),
            new HeaderNameDef(36, "AdHoc", HeaderType.AsciiText, HeaderId.AdHoc, false),
            new HeaderNameDef(36, "Content-Base", HeaderType.AsciiText, HeaderId.ContentBase, true),
            new HeaderNameDef(37, "From", HeaderType.Address, HeaderId.From, false),
            new HeaderNameDef(38, "Return-Receipt-To", HeaderType.Address, HeaderId.ReturnReceiptTo, false),
            new HeaderNameDef(38, "NNTP-Posting-Host", HeaderType.AsciiText, HeaderId.NntpPostingHost, false),
            new HeaderNameDef(40, "Lines", HeaderType.AsciiText, HeaderId.Lines, false),
            new HeaderNameDef(42, "Content-Disposition", HeaderType.ContentDisposition, HeaderId.ContentDisposition, true),
            new HeaderNameDef(42, "Thread-Topic", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(43, "Bytes", HeaderType.AsciiText, HeaderId.Bytes, false),
            new HeaderNameDef(48, "Content-ID", HeaderType.AsciiText, HeaderId.ContentId, true),
            new HeaderNameDef(48, "CC", HeaderType.Address, HeaderId.Cc, false),
            new HeaderNameDef(49, "Followup-To", HeaderType.AsciiText, HeaderId.FollowUpTo, false),
            new HeaderNameDef(52, "Reply-By", HeaderType.Date, HeaderId.ReplyBy, false),
            new HeaderNameDef(55, "BCC", HeaderType.Address, HeaderId.Bcc, false),
            new HeaderNameDef(59, "Resent-Message-ID", HeaderType.AsciiText, HeaderId.ResentMessageId, false),
            new HeaderNameDef(60, "X-OriginalArrivalTime", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(61, "X-MSMail-Priority", HeaderType.AsciiText, HeaderId.XMSMailPriority, false),
            new HeaderNameDef(64, "X-MS-Exchange-Organization-BCC", HeaderType.Address, HeaderId.XExchangeBcc, false),
            new HeaderNameDef(66, "Deferred-Delivery", HeaderType.Date, HeaderId.DeferredDelivery, false),
            new HeaderNameDef(67, "Expires", HeaderType.Date, HeaderId.Expires, false),
            new HeaderNameDef(70, "Thread-Index", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(71, "Organization", HeaderType.AsciiText, HeaderId.Organization, false),
            new HeaderNameDef(72, "Content-Transfer-Encoding", HeaderType.AsciiText, HeaderId.ContentTransferEncoding, true),
            new HeaderNameDef(78, "Encoding", HeaderType.AsciiText, HeaderId.Encoding, false),
            new HeaderNameDef(80, "X-MS-Exchange-CrossPremises-BCC", HeaderType.Address, HeaderId.XExchangeCrossPremisesBcc, false),
            new HeaderNameDef(81, "Content-Type", HeaderType.ContentType, HeaderId.ContentType, true),
            new HeaderNameDef(82, "Apparently-To", HeaderType.AsciiText, HeaderId.ApparentlyTo, false),
            new HeaderNameDef(83, "Approved", HeaderType.AsciiText, HeaderId.Approved, false),
            new HeaderNameDef(84, "X-Notes-Item", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(84, "Received", HeaderType.Received, HeaderId.Received, false),
            new HeaderNameDef(85, "Path", HeaderType.AsciiText, HeaderId.Path, false),
            new HeaderNameDef(86, "References", HeaderType.AsciiText, HeaderId.References, false),
            new HeaderNameDef(86, "Sender", HeaderType.Address, HeaderId.Sender, false),
            new HeaderNameDef(88, "Resent-To", HeaderType.Address, HeaderId.ResentTo, false),
            new HeaderNameDef(88, "X-MS-TNEF-Correlator", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(88, "Supersedes", HeaderType.AsciiText, HeaderId.Supercedes, false),
            new HeaderNameDef(89, "Resent-Reply-To", HeaderType.Address, HeaderId.ResentReplyTo, false),
            new HeaderNameDef(89, "Keywords", HeaderType.AsciiText, HeaderId.Keywords, false),
            new HeaderNameDef(91, "Control", HeaderType.AsciiText, HeaderId.Control, false),
            new HeaderNameDef(94, "In-Reply-To", HeaderType.AsciiText, HeaderId.InReplyTo, false),
            new HeaderNameDef(96, "List-Subscribe", HeaderType.AsciiText, HeaderId.ListSubscribe, false),
            new HeaderNameDef(97, "Content-Description", HeaderType.Text, HeaderId.ContentDescription, true),
            new HeaderNameDef(101, "Encrypted", HeaderType.AsciiText, HeaderId.Encrypted, false),
            new HeaderNameDef(102, "X-MS-Has-Attach", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(103, "MIME-Version", HeaderType.AsciiText, HeaderId.MimeVersion, true),
            new HeaderNameDef(103, "Article", HeaderType.AsciiText, HeaderId.Article, false),
            new HeaderNameDef(106, "Disposition-Notification-To", HeaderType.Address, HeaderId.DispositionNotificationTo, false),
            new HeaderNameDef(111, "Distribution", HeaderType.AsciiText, HeaderId.Distribution, false),
            new HeaderNameDef(111, "Date", HeaderType.Date, HeaderId.Date, false),
            new HeaderNameDef(117, "Xref", HeaderType.AsciiText, HeaderId.XRef, false),
            new HeaderNameDef(117, "X-MimeOLE", HeaderType.AsciiText, HeaderId.Unknown, false),
            new HeaderNameDef(118, "Sensitivity", HeaderType.AsciiText, HeaderId.Sensitivity, false),
            new HeaderNameDef(118, "To", HeaderType.Address, HeaderId.To, false),
            new HeaderNameDef(118, "Reply-To", HeaderType.Address, HeaderId.ReplyTo, false),
            new HeaderNameDef(119, "Expiry-Date", HeaderType.Date, HeaderId.ExpiryDate, false),
            new HeaderNameDef(120, "Priority", HeaderType.AsciiText, HeaderId.Priority, false),
            new HeaderNameDef(121)
        };

        public static HeaderNameIndex[] nameHashTable;
        public static HeaderNameIndex[] nameIndex;
        public static int[] valueHashTable;
        public static HeaderValueDef[] values;


        public struct HeaderNameDef {

            public HeaderNameDef(int hash) {
                this.hash = (byte) hash;
                restricted = false;
                headerType = HeaderType.Text;
                name = null;
                publicHeaderId = HeaderId.Unknown;
            }

            public HeaderNameDef(int hash, string name, HeaderType headerType, HeaderId publicHeaderId, bool restricted) {
                this.hash = (byte) hash;
                this.restricted = restricted;
                this.headerType = headerType;
                this.name = name;
                this.publicHeaderId = publicHeaderId;
            }

            public byte hash;
            public HeaderType headerType;
            public string name;
            public HeaderId publicHeaderId;
            public bool restricted;

        }


        public struct HeaderValueDef {

            public HeaderValueDef(int hash) {
                this.hash = (byte) hash;
                value = null;
            }

            public HeaderValueDef(int hash, string value) {
                this.hash = (byte) hash;
                this.value = value;
            }

            public byte hash;
            public string value;

        }

    }

}