using Columbidae.Message;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;

namespace Columbidae.Server.Core.Service.Impl;

using CMsg = Columbidae.Message.Message;
using FrameType = Frame.Types.FrameType;

public abstract class MessageDbContext : DbContext
{
    public DbSet<MessageStore> Messages { get; set; }
    public DbSet<FrameStoreBase> Frames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmojiFrameStore>().ToTable("EmojiFrames");
        modelBuilder.Entity<FileFrameStore>().ToTable("FileFrames");
        modelBuilder.Entity<ReplyFrameStore>().ToTable("ReplyFrames");
        modelBuilder.Entity<ImageFrameStore>().ToTable("ImageFrames");
        modelBuilder.Entity<JsonFrameStore>().ToTable("JsonFrames");
        modelBuilder.Entity<MentionFrameStore>().ToTable("MentionFrames");
        modelBuilder.Entity<ForwardFrameStore>().ToTable("ForwardFrames");
        modelBuilder.Entity<TextFrameStore>().ToTable("TextFrames");
        modelBuilder.Entity<VideoFrameStore>().ToTable("VideoFrames");
        modelBuilder.Entity<XmlFrameStore>().ToTable("XmlFrames");

        modelBuilder.HasSequence<ulong>("FrameSequence").StartsAt(1);
        modelBuilder.Entity<FrameStoreBase>()
            .Property(s => s.Id)
            .HasDefaultValueSql("NEXT VALUE FOR FrameSequence");
    }

    #region Data Models

    public class MessageStore
    {
        public ulong Id { get; set; }
        public DateTime Time { get; set; }
        public List<FrameStoreBase> Frames { get; set; }
        public CMsg.Types.MessageType Type { get; set; }
        public uint SenderId { get; set; }
        public uint DestinationId { get; set; }
        public uint? GroupId { get; set; }

        public IEnumerable<Frame> GetFrames()
        {
            return Frames.Select(store => store.ToFrame());
        }

        public CMsg ToMessage()
        {
            var cmsg = new CMsg
            {
                Id = Id,
                Time = Timestamp.FromDateTime(Time),
                Type = Type,
                Sender = SenderId,
                Destination = DestinationId,
                Frames = { GetFrames() }
            };
            if (GroupId.HasValue) cmsg.Group = GroupId.Value;

            return cmsg;
        }
    }

    public abstract class FrameStoreBase
    {
        public ulong Id { get; set; }

        public abstract Frame ToFrame();
    }

    public class EmojiFrameStore : FrameStoreBase
    {
        public ushort EmojiId { get; set; }
        public bool IsLarge { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Emoji,
                Emoji = new EmojiFrame
                {
                    Emoji = EmojiId,
                    IsLarge = IsLarge
                }
            };
        }
    }

    public class FileFrameStore : FrameStoreBase
    {
        public long Size { get; set; }
        public string Name { get; set; }
        public byte[] Md5 { get; set; }
        public string? Url { get; set; }
        public string Uuid { get; set; }
        public string Hash { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.File,
                File = new FileFrame
                {
                    Size = Size,
                    Name = Name,
                    Md5 = ByteString.CopyFrom(Md5),
                    Url = Url,
                    Uuid = Uuid,
                    Hash = Hash
                }
            };
        }
    }

    public class ReplyFrameStore : FrameStoreBase
    {
        public DateTime Time { get; set; }
        public ulong ReplySeq { get; set; }
        public string? SenderUid { get; set; }
        public uint ReplyUin { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Reply,
                Reply = new ReplyFrame
                {
                    ReplySeq = ReplySeq,
                    ReplyUin = ReplyUin,
                    SenderUid = SenderUid
                }
            };
        }
    }

    public class ImageFrameStore : FrameStoreBase
    {
        public uint Width { get; set; }
        public uint Height { get; set; }
        public uint Size { get; set; }
        public string Url { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Picture,
                Image = new ImageFrame
                {
                    Height = Height,
                    Width = Width,
                    Size = Size,
                    Url = Url
                }
            };
        }
    }

    public class JsonFrameStore : FrameStoreBase
    {
        public string Json { get; set; }
        public string ResId { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Json,
                Json = new JsonFrame
                {
                    Json = Json,
                    ResId = ResId
                }
            };
        }
    }

    public class MentionFrameStore : FrameStoreBase
    {
        public uint Uin { get; set; }
        public string Uid { get; set; }
        public string Nick { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Mention,
                Mention = new MentionFrame
                {
                    Uin = Uin,
                    Uid = Uid,
                    Nick = Nick
                }
            };
        }
    }

    public class ForwardFrameStore : FrameStoreBase
    {
        public uint GroupUin { get; set; }
        public List<MessageStore> Messages { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Forward,
                Forward = new ForwardFrame
                {
                    GroupUin = GroupUin,
                    Messages = { Messages.Select(store => store.ToMessage()) }
                }
            };
        }
    }

    public class TextFrameStore : FrameStoreBase
    {
        public string Text { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Text,
                Text = new TextFrame
                {
                    Text = Text
                }
            };
        }
    }

    public class VideoFrameStore : FrameStoreBase
    {
        public uint Height { get; set; }
        public uint Width { get; set; }
        public uint Size { get; set; }
        public string Url { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Video,
                Video = new VideoFrame
                {
                    Height = Height,
                    Width = Width,
                    Size = Size,
                    Url = Url
                }
            };
        }
    }

    public class XmlFrameStore : FrameStoreBase
    {
        public string Xml { get; set; }

        public override Frame ToFrame()
        {
            return new Frame
            {
                Type = FrameType.Xml,
                Xml = new XmlFrame
                {
                    Xml = Xml
                }
            };
        }
    }

    #endregion
}

public static class Convert
{
    public static MessageDbContext.MessageStore ToMessageStore(this CMsg message)
    {
        return new MessageDbContext.MessageStore
        {
            Id = message.Id,
            Time = message.Time.ToDateTime(),
            Frames = message.Frames.Select(f => f.ToFrameStore()).ToList(),
            Type = message.Type,
            SenderId = message.Sender,
            DestinationId = message.Destination,
            GroupId = message.Destination
        };
    }

    public static MessageDbContext.FrameStoreBase ToFrameStore(this Frame frame)
    {
        switch (frame.Type)
        {
            case FrameType.Emoji:
                return new MessageDbContext.EmojiFrameStore
                {
                    EmojiId = (ushort)frame.Emoji.Emoji,
                    IsLarge = frame.Emoji.IsLarge,
                    Id = frame.Id
                };
            case FrameType.File:
                return new MessageDbContext.FileFrameStore
                {
                    Name = frame.File.Name,
                    Hash = frame.File.Hash,
                    Size = frame.File.Size,
                    Md5 = frame.File.Md5.ToByteArray(),
                    Url = frame.File.Url,
                    Uuid = frame.File.Uuid,
                    Id = frame.Id
                };
            case FrameType.Reply:
                return new MessageDbContext.ReplyFrameStore
                {
                    ReplySeq = frame.Reply.ReplySeq,
                    ReplyUin = frame.Reply.ReplyUin,
                    SenderUid = frame.Reply.SenderUid,
                    Time = frame.Reply.Time.ToDateTime(),
                    Id = frame.Id
                };
            case FrameType.Picture:
                return new MessageDbContext.ImageFrameStore
                {
                    Height = frame.Image.Height,
                    Width = frame.Image.Width,
                    Size = frame.Image.Size,
                    Url = frame.Image.Url,
                    Id = frame.Id
                };
            case FrameType.Mention:
                return new MessageDbContext.MentionFrameStore
                {
                    Nick = frame.Mention.Nick,
                    Uin = frame.Mention.Uin,
                    Uid = frame.Mention.Uid,
                    Id = frame.Id
                };
            case FrameType.Forward:
                return new MessageDbContext.ForwardFrameStore
                {
                    GroupUin = frame.Forward.GroupUin,
                    Messages = frame.Forward.Messages.Select(s => s.ToMessageStore()).ToList(),
                    Id = frame.Id
                };
            case FrameType.Text:
                return new MessageDbContext.TextFrameStore
                {
                    Text = frame.Text.Text,
                    Id = frame.Id
                };
            case FrameType.Video:
                return new MessageDbContext.VideoFrameStore
                {
                    Height = frame.Video.Height,
                    Width = frame.Video.Width,
                    Size = frame.Video.Size,
                    Url = frame.Video.Url,
                    Id = frame.Id
                };
            case FrameType.Xml:
                return new MessageDbContext.XmlFrameStore
                {
                    Xml = frame.Xml.Xml,
                    Id = frame.Id
                };
            case FrameType.Json:
                return new MessageDbContext.JsonFrameStore
                {
                    Json = frame.Json.Json,
                    ResId = frame.Json.ResId,
                    Id = frame.Id
                };
            default:
                throw new InvalidDataException($"Frame type {frame.Type} not supported");
        }
    }
}