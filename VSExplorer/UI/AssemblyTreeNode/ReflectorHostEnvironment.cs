using Microsoft.Cci;
using System;

namespace AndersLiu.Reflector.Core
{
    public class ReflectorHostEnvironment : MetadataReaderHost
    {
        public override IUnit LoadUnitFrom(string location)
        {
            try
            {
                var binDoc = BinaryDocument.GetBinaryDocumentForFile(location, this);
                var unit = _reader.OpenModule(binDoc);

                this.RegisterAsLatest(unit);

                return unit;
            }
            catch (Exception e) { }

            return null;
        }

        public IUnit LoadUnitFroms(string location)
        {
            var binDoc = BinaryDocument.GetBinaryDocumentForFile(location, this);
            var unit = _reader.OpenModule(binDoc);

            this.RegisterAsLatest(unit);

            return unit;
        }

        public void CloseReader()
        {
            _reader = null;
        }

        public ReflectorHostEnvironment()
        {
            _reader = new PeReader(this);
        }

        private PeReader _reader;
    }
}