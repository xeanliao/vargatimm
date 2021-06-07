using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GPS.Tool.Data;
using GPS.Tool.Mapping;

namespace GPS.Tool.SelectMapping
{
    abstract class MakerBase
    {
        public delegate void MessageHandler(int total, int current, int innerCount, string code, bool inner, bool completed);
        public event MessageHandler Messaging;
        private Thread thread;

        public void StartMake()
        {
            thread = new Thread(new ThreadStart(MakeMappings));
            thread.Start();
        }

        protected abstract void MakeMappings();

        protected void SendMessage(int total, int current, int innerCount, string code, bool inner, bool completed)
        {
            if (Messaging != null)
            {
                Messaging(total, current, innerCount, code, inner, completed);
            }
        }
    }

    abstract class Maker<T, M, C> : MakerBase
    {
        protected int _count;
        protected int _current;
        protected int _innerCount;
        ZipRelationQueue _zipRelationQueue;

        public Maker()
        {
        }

        protected abstract int GetItemsCount();
        protected abstract List<M> GetMappings(T t);
        protected abstract List<ICoordinate> GetShape(T t);
        protected abstract List<T> GetItems(int skip, int count);
        protected abstract void MappingItems(List<T> items);

        protected override void MakeMappings()
        {
            _count = GetItemsCount();
            _current = 0;
            _innerCount = 0;
            _zipRelationQueue = new ZipRelationQueue();
            int number = 100;
            int numberCount = _count % number > 0 ? _count / number + 1 : _count / number;
            SendMessage("start", false);
            for (int i = 0; i < numberCount; i++)
            {
                MappingItems(GetItems(i * number, number));
            }
            SendMessage("End", false, true);
        }

        protected void SendMessage(string code, bool inner)
        {
            SendMessage(code, inner, false);
        }

        protected void SendMessage(string code, bool inner, bool completed)
        {
            SendMessage(_count, _current, _innerCount, code, inner, completed);
        }
        protected List<ThreeZipArea> GetThreeZipAreas(string code)
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.ThreeZipAreas.Where(t => t.Code == code).OrderBy(t => t.Id).ToList();
        }

        protected List<FiveZipArea> GetFiveZipAreas(string code)
        {
            AreaDataContext dataContext = new AreaDataContext();
            return dataContext.FiveZipAreas.Where(t => t.Code == code).ToList();
        }

        public ZipRelation GetZipRelation(FiveZipArea fiveZip, List<ICoordinate> coordinates)
        {
            ZipRelation relation = _zipRelationQueue.Get(fiveZip.Id);
            if (relation == null)
            {
                List<int> threeZipIds = new List<int>();
                List<ThreeZipArea> threeZips = GetThreeZipAreas(fiveZip.Code.Substring(0, 3));
                foreach (ThreeZipArea threeZip in threeZips)
                {
                    List<ICoordinate> threeShape = GetShape(threeZip.ThreeZipAreaCoordinates.OrderBy(t => t.Id).ToList());

                    if (ShapeMethods.PolygonInPolygon(threeShape, coordinates))
                    {
                        threeZipIds.Add(threeZip.Id);
                    }
                }

                relation = new ZipRelation()
                {
                    FiveZipId = fiveZip.Id,
                    ThreeZipIds = threeZipIds
                };
                _zipRelationQueue.Add(relation);
            }
            //ThreeZipArea threeZip 
            return relation;
        }

        protected List<ICoordinate> GetShape(List<FiveZipAreaCoordinate> coordinates)
        {
            List<ICoordinate> shape = new List<ICoordinate>();
            foreach (FiveZipAreaCoordinate coordinate in coordinates)
            {
                shape.Add(coordinate);
            }

            return shape;
        }

        protected Dictionary<int, List<ICoordinate>> GetShapes(List<FiveZipAreaCoordinate> coordinates)
        {
            Dictionary<int, List<ICoordinate>> shapes = new Dictionary<int, List<ICoordinate>>();
            int shapeId = 0;
            if (coordinates.Count > 0)
            {
                shapeId = coordinates[0].ShapeId;
                shapes.Add(shapeId, new List<ICoordinate>());
            }
            foreach (FiveZipAreaCoordinate coordinate in coordinates)
            {
                if (coordinate.ShapeId != shapeId)
                {
                    shapeId = coordinate.ShapeId;
                    shapes.Add(shapeId, new List<ICoordinate>());
                }
                shapes[shapeId].Add(coordinate);
            }

            return shapes;
        }

        protected List<ICoordinate> GetShape(List<ThreeZipAreaCoordinate> coordinates)
        {
            List<ICoordinate> shape = new List<ICoordinate>();
            foreach (ThreeZipAreaCoordinate coordinate in coordinates)
            {
                shape.Add(coordinate);
            }
            return shape;
        }

        protected Dictionary<int, List<ICoordinate>> GetShapes(List<ThreeZipAreaCoordinate> coordinates)
        {
            Dictionary<int, List<ICoordinate>> shapes = new Dictionary<int, List<ICoordinate>>();
            int shapeId = 0;
            if (coordinates.Count > 0)
            {
                shapeId = coordinates[0].ShapeId;
                shapes.Add(shapeId, new List<ICoordinate>());
            }
            foreach (ThreeZipAreaCoordinate coordinate in coordinates)
            {
                if (coordinate.ShapeId != shapeId)
                {
                    shapeId = coordinate.ShapeId;
                    shapes.Add(shapeId, new List<ICoordinate>());
                }
                shapes[shapeId].Add(coordinate);
            }

            return shapes;
        }
    }

    class ThreeZipRelation
    {
        public int ThreeZipId { get; set; }
        //public int ThreeZipShapeId { get; set; }
    }

    class ZipRelation
    {
        public int FiveZipId { get; set; }
        //public int FiveZipShapeId { get; set; }
        public List<int> ThreeZipIds { get; set; }
    }

    class ZipRelationQueue
    {
        Queue<ZipRelation> _queue;
        int _maxLength = 1000;

        public ZipRelationQueue()
        {
            _queue = new Queue<ZipRelation>();
        }

        public ZipRelationQueue(int maxLength)
        {
            _queue = new Queue<ZipRelation>();
            _maxLength = maxLength;
        }

        public ZipRelation Get(int fiveZipId)
        {
            ZipRelation ret = null;
            foreach (ZipRelation relation in _queue)
            {
                if (relation.FiveZipId == fiveZipId)
                {
                    ret = relation;
                    break;
                }
            }
            return ret;
        }

        public void Add(ZipRelation relation)
        {
            if (_queue.Count >= _maxLength)
            {
                _queue.Dequeue();
            }
            _queue.Enqueue(relation);
        }

    }
}
