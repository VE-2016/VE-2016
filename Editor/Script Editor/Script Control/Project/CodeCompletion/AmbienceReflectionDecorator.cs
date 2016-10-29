using AIMS.Libraries.Scripting.Dom;

namespace AIMS.Libraries.Scripting.CodeCompletion
{
    /// <summary>
    /// This class wraps a ILanguageConversion to System.Reflection
    /// </summary>
    public class AmbienceReflectionDecorator : IAmbience
    {
        private IAmbience _conv;

        public ConversionFlags ConversionFlags
        {
            get
            {
                return _conv.ConversionFlags;
            }
            set
            {
                _conv.ConversionFlags = value;
            }
        }

        public string Convert(ModifierEnum modifier)
        {
            return _conv.Convert(modifier);
        }

        public string Convert(IClass c)
        {
            return _conv.Convert(c);
        }

        public string ConvertEnd(IClass c)
        {
            return _conv.ConvertEnd(c);
        }

        public string Convert(IField field)
        {
            return _conv.Convert(field);
        }

        public string Convert(IProperty property)
        {
            return _conv.Convert(property);
        }

        public string Convert(IEvent e)
        {
            return _conv.Convert(e);
        }

        public string Convert(IMethod m)
        {
            return _conv.Convert(m);
        }

        public string ConvertEnd(IMethod m)
        {
            return _conv.ConvertEnd(m);
        }

        public string Convert(IParameter param)
        {
            return _conv.Convert(param);
        }

        public string Convert(IReturnType returnType)
        {
            return _conv.Convert(returnType);
        }

        public AmbienceReflectionDecorator(IAmbience conv)
        {
            if (conv == null)
            {
                throw new System.ArgumentNullException("conv");
            }
            _conv = conv;
        }

        public string WrapAttribute(string attribute)
        {
            return _conv.WrapAttribute(attribute);
        }

        public string WrapComment(string comment)
        {
            return _conv.WrapComment(comment);
        }

        public string GetIntrinsicTypeName(string dotNetTypeName)
        {
            return _conv.GetIntrinsicTypeName(dotNetTypeName);
        }
    }
}