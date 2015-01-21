using System.Threading.Tasks;

namespace MediatR.Extensions.Autofac.Tests
{
    public class NoteHandler : INotificationHandler<Note>
    {
        public void Handle(Note notification)
        {
            notification.Count++;
        }
    }


    public class GenericNotificationHandler : INotificationHandler<INotificationWithCount>
    {
        public void Handle(INotificationWithCount notification)
        {
            notification.Count++;
        }
    }

    public class AsyncNoteHandler : IAsyncNotificationHandler<Note>
    {
        public async Task Handle(Note notification)
        {
            await Task.Run(() => notification.Count++);
        }
    }

    public class AnotherNoteHandler : INotificationHandler<Note>
    {
        public void Handle(Note notification)
        {
            notification.Count++;
        }
    }
    
    public class Note : INotificationWithCount
    {
        public int Count { get; set; }
    }
}