using System;
using System.Text;

namespace hashes
{
    public class GhostsTask :
        IFactory<Document>, IFactory<Vector>, IFactory<Segment>, IFactory<Cat>, IFactory<Robot>, IMagic
    {
        private Cat _cat;
        private Robot _robot;
        private Vector _vector;
        private Segment _segment;
        private Document _document;

        private byte[] _content;

        public GhostsTask()
        {
            _cat = new Cat("Barsik", "Persian", new DateTime(2020, 1, 1));

            _robot = new Robot("R2D2", 42);

            _vector = new Vector(1, 1);

            _segment = new Segment(new Vector(1, 1), new Vector(1, 2));

            _content = new byte[] { 1, 2, 3 };
            _document = new Document("Title", Encoding.UTF8, _content);
        }

        public void DoMagic()
        {
            _cat.Rename("GhostCat");

            Robot.BatteryCapacity++;

            _vector.Add(new Vector(10, 10));
            _segment.End.Add(new Vector(100, 100));

            if (_content.Length > 0)
                _content[0] = 255;
        }

        Vector IFactory<Vector>.Create() => _vector;

        Segment IFactory<Segment>.Create() => _segment;

        Document IFactory<Document>.Create() => _document;  

        Cat IFactory<Cat>.Create() => _cat;

        Robot IFactory<Robot>.Create() => _robot;
    }
}