using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GPS.DomainLayer.Entities {
    [DataContract(Namespace = "TIMM.DataLayer")]
    public class Record {
        private int _classification;
        private int _areaId;
        private bool _value;

        public Record() { }

        public Record(int classification, int areaId, bool value) {
            _classification = classification;
            _areaId = areaId;
            _value = value;
        }

        [DataMember]
        public int Classification {
            get { return this._classification; }
            set { this._classification = value; }
        }

        [DataMember]
        public int AreaId {
            get { return this._areaId; }
            set { this._areaId = value; }
        }

        [DataMember]
        public bool Value {
            get { return this._value; }
            set { this._value = value; }
        }
    }
}
