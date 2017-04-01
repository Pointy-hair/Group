using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using RevolutionaryStuff.Core.Crypto;
using RevolutionaryStuff.Core.Collections;

namespace RevolutionaryStuff.SSIS
{
    [DtsPipelineComponent(
        DisplayName = "The Matcher", 
        ComponentType = ComponentType.Transform,
        SupportsBackPressure = true,
        IconResource = "RevolutionaryStuff.SSIS.Resources.Icon1.ico")]
    public class MatcherTransformComponent : PipelineComponent
    {
        private static class PropertyNames
        {
            public static class Input
            {
                public const string Root = "Root";
                public const int RootId = 0;
                public const string Comparison = "Comparison";
                public const int ComparisonId = 1;
            }
            public static class Output
            {
                public const string Matches = "Matches";
                public const string Orphans = "Orphans";
            }
        }

        public MatcherTransformComponent()
        {
#if false
            for (int z=0;z<60;++z)
            {
                System.Threading.Thread.Sleep(1000);
            }
#endif
        }

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();
            base.RemoveAllInputsOutputsAndCustomProperties();

            ComponentMetaData.Name = "The Matcher";
            ComponentMetaData.Description = "A SSIS Data Flow Transformation Component that auto joins 2 inputs (based on field name / data type) and returns the matched (union of all columns, same rows as an inner join) and orphanned (* from the left table and unmatched rows from left table) tables.";
            ComponentMetaData.ContactInfo = "jason@jasonthomas.com";

            var left = ComponentMetaData.InputCollection.New();
            left.Name = PropertyNames.Input.Root;
            var right = ComponentMetaData.InputCollection.New();
            right.Name = PropertyNames.Input.Comparison;
            var matched = ComponentMetaData.OutputCollection.New();
            matched.SynchronousInputID = 0;
            matched.Name = PropertyNames.Output.Matches;
            matched.Description = "Root rows that have have corresponding matches in the Comparison";
            var notMatched = ComponentMetaData.OutputCollection.New();
            notMatched.SynchronousInputID = 0;
            notMatched.Name = PropertyNames.Output.Orphans;
            notMatched.Description = "Root rows that have no corresponding matches in the Comparison";
        }

        public override IDTSCustomProperty100 SetComponentProperty(string propertyName, object propertyValue)
        {
            switch (propertyName)
            {
                case PropertyNames.Output.Matches:
                    AddOutputColumns(ComponentMetaData.InputCollection[0].InputColumnCollection, ComponentMetaData.OutputCollection[0].OutputColumnCollection);
                    break;
                case PropertyNames.Output.Orphans:
                    AddOutputColumns(ComponentMetaData.InputCollection[0].InputColumnCollection, ComponentMetaData.OutputCollection[1].OutputColumnCollection);
                    break;
            }
            return base.SetComponentProperty(propertyName, propertyValue);
        }

        public override IDTSInputColumn100 SetUsageType(int inputID, IDTSVirtualInput100 virtualInput, int lineageID, DTSUsageType usageType)
        {
            return base.SetUsageType(inputID, virtualInput, lineageID, usageType);
        }

        public override IDTSCustomProperty100 SetInputProperty(int inputID, string propertyName, object propertyValue)
        {
            return base.SetInputProperty(inputID, propertyName, propertyValue);
        }

        public override IDTSCustomProperty100 SetInputColumnProperty(int inputID, int inputColumnID, string propertyName, object propertyValue)
        {
            return base.SetInputColumnProperty(inputID, inputColumnID, propertyName, propertyValue);
        }

        public override void OnInputPathAttached(int inputID)
        {
            base.OnInputPathAttached(inputID);
            if (this.ComponentMetaData.InputCollection[0].IsAttached && 
                this.ComponentMetaData.InputCollection[1].IsAttached)
            {
                DefineOutputs();
            }
        }

        public override void OnOutputPathAttached(int outputID)
        {
            base.OnOutputPathAttached(outputID);
        }

        private IList<string> GetComparisonColumnKeys()
        {
            var leftCols = ComponentMetaData.InputCollection[0].InputColumnCollection;
            var rightCols = ComponentMetaData.InputCollection[1].InputColumnCollection;
            var leftDefs = new HashSet<string>();
            var rightDefs = new HashSet<string>();
            for (int z = 0; z < leftCols.Count; ++z)
            {
                leftDefs.Add(CreateColumnFingerprint(leftCols[z]));
            }
            for (int z = 0; z < rightCols.Count; ++z)
            {
                rightDefs.Add(CreateColumnFingerprint(rightCols[z]));
            }
            var commonDefs = new HashSet<string>(leftDefs.Intersect(rightDefs));
            return commonDefs.ToList();
        }

        private void DefineOutputs()
        {
            for (int z = 0; z < 2; ++z)
            {
                var input = ComponentMetaData.InputCollection[z].GetVirtualInput();
                foreach (IDTSVirtualInputColumn100 vcol in input.VirtualInputColumnCollection)
                {
                    input.SetUsageType(vcol.LineageID, DTSUsageType.UT_READONLY);
                }
            }
            var leftCols = ComponentMetaData.InputCollection[0].InputColumnCollection;
            var rightCols = ComponentMetaData.InputCollection[1].InputColumnCollection;
            var commonDefs = GetComparisonColumnKeys();
            var matchedOutputColumns = ComponentMetaData.OutputCollection[0].OutputColumnCollection;
            matchedOutputColumns.RemoveAll();
            AddOutputColumns(leftCols, matchedOutputColumns);
            for (int z = 0; z < rightCols.Count; ++z)
            {
                var col = rightCols[z];
                if (!commonDefs.Contains(CreateColumnFingerprint(col)))
                {
                    CopyColumnDefinition(matchedOutputColumns, col);
                }
            }
            var orphanOutputColumns = ComponentMetaData.OutputCollection[1].OutputColumnCollection;
            orphanOutputColumns.RemoveAll();
            AddOutputColumns(leftCols, orphanOutputColumns);
        }

        public override DTSValidationStatus Validate()
        {
            var ret = base.Validate();
            switch (ret)
            {
                case DTSValidationStatus.VS_ISVALID:
                    if (!ComponentMetaData.InputCollection[0].IsAttached || !ComponentMetaData.InputCollection[0].IsAttached)
                    {
                        ret = DTSValidationStatus.VS_ISBROKEN;
                    }
                    else
                    {
                        var leftCols = ComponentMetaData.InputCollection[0].InputColumnCollection;
                        var rightCols = ComponentMetaData.InputCollection[1].InputColumnCollection;
                        var commonDefs = GetComparisonColumnKeys();
                        var matchedOutputColumns = ComponentMetaData.OutputCollection[0].OutputColumnCollection;
                        var orphanOutputColumns = ComponentMetaData.OutputCollection[1].OutputColumnCollection;
                        if (matchedOutputColumns.Count != leftCols.Count + rightCols.Count - commonDefs.Count)
                        {
                            ret = DTSValidationStatus.VS_NEEDSNEWMETADATA;
                        }
                        else if (orphanOutputColumns.Count != leftCols.Count)
                        {
                            ret = DTSValidationStatus.VS_NEEDSNEWMETADATA;
                        }
                    }
                    break;
            }
            return ret;
        }

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();
            DefineOutputs();
        }

        private class ColumnBufferMapping
        {
            public IList<int> ByColumnPosition = new List<int>();
            public IDictionary<string, int> ByColumnName = new Dictionary<string, int>();

            public void Add(string columnName, int offset)
            {
                ByColumnPosition.Add(offset);
                ByColumnName[columnName] = offset;
            }
        }

        private ColumnBufferMapping InputRootBufferColumnIndicees;
        private ColumnBufferMapping InputComparisonBufferColumnIndicees;
        private ColumnBufferMapping OutputMatchesBufferColumnIndicees;
        private ColumnBufferMapping OutputOrphansBufferColumnIndicees;

        public override void PreExecute()
        {
            base.PreExecute();
            InputRootBufferColumnIndicees = GetBufferColumnIndicees(ComponentMetaData.InputCollection[0]);
            InputComparisonBufferColumnIndicees = GetBufferColumnIndicees(ComponentMetaData.InputCollection[1]);
            OutputMatchesBufferColumnIndicees = GetBufferColumnIndicees(ComponentMetaData.OutputCollection[0]);
            OutputOrphansBufferColumnIndicees = GetBufferColumnIndicees(ComponentMetaData.OutputCollection[1]);
        }

        private ColumnBufferMapping GetBufferColumnIndicees(IDTSInput100 input)
        {
            var cbm = new ColumnBufferMapping();
            for (int x = 0; x < input.InputColumnCollection.Count; x++)
            {
                var column = input.InputColumnCollection[x];
                var offset = BufferManager.FindColumnByLineageID(input.Buffer, column.LineageID);
                cbm.Add(column.Name, offset);
            }

            return cbm;
        }

        private ColumnBufferMapping GetBufferColumnIndicees(IDTSOutput100 output)
        {
            var cbm = new ColumnBufferMapping();
            for (int x = 0; x < output.OutputColumnCollection.Count; x++)
            {
                var column = output.OutputColumnCollection[x];
                var offset = BufferManager.FindColumnByLineageID(output.Buffer, column.LineageID);
                cbm.Add(column.Name, offset);
            }
            return cbm;
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            var input = ComponentMetaData.InputCollection.GetObjectByID(inputID);
            switch (input.Name)
            {
                case PropertyNames.Input.Root:
                    if (!InputRootProcessed)
                    {
                        ProcessInputRoot(input, buffer);
                    }
                    break;
                case PropertyNames.Input.Comparison:
                    if (!InputComparisonProcessed)
                    {
                        ProcessInputComparison(input, buffer);
                    }
                    break;
                default:
                    bool fireAgain = true;
                    ComponentMetaData.FireInformation(0, "", string.Format("Not expecting inputID={0}", inputID), "", 0, ref fireAgain);
                    throw new InvalidOperationException(string.Format("Not expecting inputID={0}", inputID));
            }
        }

        private class Fingerprinter
        {
            private readonly StringBuilder Strings = new StringBuilder(8000);

            public void AppendValue(object o)
            {
                Strings.AppendLine((o ?? "").ToString());
            }

            public void Clear()
            {
                Strings.Clear();
            }

            public string GetFingerPrint()
            {
                var s = Strings.ToString();
                if (s.Length < 100) return s;
                var buf = Encoding.Unicode.GetBytes(s);
                var hash = Hash.Compute(buf, Hash.CommonHashAlgorithmNames.Sha1).Urn;
                return hash;
            }
        }

        private void ProcessInputRoot(IDTSInput100 input, PipelineBuffer buffer)
        {
            var matchAttached = ComponentMetaData.OutputCollection[0].IsAttached;
            var matchedCC = ComponentMetaData.OutputCollection[0].OutputColumnCollection;
            var orphansAttached = ComponentMetaData.OutputCollection[1].IsAttached;
            var orhpannedCC = ComponentMetaData.OutputCollection[1].OutputColumnCollection;
            var commonFingerprints = GetComparisonColumnKeys();
            var fingerprinter = new Fingerprinter();
            var sourceVals = new List<object>();
            int rowsProcessed = 0;
            while (buffer.NextRow())
            {
                for (int z = 0; z < input.InputColumnCollection.Count; ++z)
                {
                    var col = input.InputColumnCollection[z];
                    var colFingerprint = CreateColumnFingerprint(col);
                    var o = GetObject(col.Name, col.DataType, z, buffer, InputComparisonBufferColumnIndicees);
                    if (commonFingerprints.Contains(colFingerprint))
                    {
                        fingerprinter.AppendValue(o);
                    }
                    sourceVals.Add(o);
                }
                var fingerprint = fingerprinter.GetFingerPrint();
                if (AppendsByCommonFieldHash.ContainsKey(fingerprint))
                {
                    if (matchAttached)
                    {
                        foreach (var appends in AppendsByCommonFieldHash[fingerprint])
                        {
                            CopyToMatchedOutput(matchedCC, sourceVals, appends);
                        }
                    }
                }
                else
                {
                    if (orphansAttached)
                    {
                        CopyToOrphannedOutput(orhpannedCC, sourceVals);
                    }
                }
                fingerprinter.Clear();
                sourceVals.Clear();
                ++rowsProcessed;
            }
            if (buffer.EndOfRowset)
            {
                MatchedOutputBuffer.SetEndOfRowset();
                OrphannedOutputBuffer.SetEndOfRowset();
                InputRootProcessed = true;
            }
        }

        public override void IsInputReady(int[] inputIDs, ref bool[] canProcess)
        {
            for (int i = 0; i < inputIDs.Length; i++)
            {
                int inputIndex = ComponentMetaData.InputCollection.GetObjectIndexByID(inputIDs[i]);
                bool can;
                switch (inputIndex)
                {
                    case PropertyNames.Input.RootId:
                        can = InputRootProcessed || InputComparisonProcessed;
                        break;
                    case PropertyNames.Input.ComparisonId:
                        can = true; //!InputComparisonProcessed;
                        break;
                    default:
                        can = false;
                        break;
                }
                canProcess[i] = can;
            }
        }

        private void CopyToMatchedOutput(IDTSOutputColumnCollection100 cc, IList<object> sources, IList<object> appends)
        {
            var buf = MatchedOutputBuffer;
            buf.AddRow();
            CopyValsToBuffer(buf, cc, sources, 0, OutputMatchesBufferColumnIndicees);
            CopyValsToBuffer(buf, cc, appends, sources.Count, OutputMatchesBufferColumnIndicees);
        }

        private void CopyToOrphannedOutput(IDTSOutputColumnCollection100 cc, IList<object> sources)
        {
            var buf = OrphannedOutputBuffer;
            buf.AddRow();
            CopyValsToBuffer(buf, cc, sources, 0, OutputOrphansBufferColumnIndicees);
        }

        private void CopyValsToBuffer(PipelineBuffer buf, IDTSOutputColumnCollection100 cc, IList<object> vals, int offset, ColumnBufferMapping cbm)
        {
            for (int z = 0; z < vals.Count; ++z)
            {
                var col = cc[z + offset];
                var i = cbm.ByColumnPosition[z+offset];
                var o = vals[z];
                SetObject(buf, col.DataType, i, o);
            }            
        }

        private static void SetObject(PipelineBuffer buf, DataType dt, int i, object val)
        {
            if (val == null)
            {
                buf.SetNull(i);
            }
            else
            {
                switch (dt)
                {
                    case DataType.DT_GUID:
                        buf.SetGuid(i, (Guid)val);
                        break;
                    case DataType.DT_BOOL:
                        buf.SetBoolean(i, (bool)val);
                        break;
                    case DataType.DT_UI2:
                        buf.SetUInt16(i, (System.UInt16)val);
                        break;
                    case DataType.DT_UI4:
                        buf.SetUInt32(i, (System.UInt32)val);
                        break;
                    case DataType.DT_UI8:
                        buf.SetUInt64(i, (System.UInt64)val);
                        break;
                    case DataType.DT_I1:
                        buf.SetByte(i, (byte)val);
                        break;
                    case DataType.DT_I2:
                        buf.SetInt16(i, (System.Int16)val);
                        break;
                    case DataType.DT_I4:
                        buf.SetInt32(i, (System.Int32)val);
                        break;
                    case DataType.DT_I8:
                        buf.SetInt64(i, (System.Int64)val);
                        break;
                    case DataType.DT_WSTR:
                    case DataType.DT_STR:
                    case DataType.DT_TEXT:
                        buf.SetString(i, (string)val);
                        break;
                    case DataType.DT_DATE:
                        buf.SetDate(i, (DateTime)val);
                        break;
                    case DataType.DT_DECIMAL:
                        buf.SetDecimal(i, (decimal)val);
                        break;
                    default:
                        buf.SetNull(i);
                        break;
                }
            }
        }

        PipelineBuffer MatchedOutputBuffer;
        PipelineBuffer OrphannedOutputBuffer;

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            if (buffers.Length == 2)
            {
                MatchedOutputBuffer = buffers[0];
                OrphannedOutputBuffer = buffers[1];
            }
        }


        public override void PrepareForExecute()
        {
            base.PrepareForExecute();
            AppendsByCommonFieldHash.Clear();
            InputRootProcessed = false;
            InputComparisonProcessed = false;
        }

        private object GetObject(string colName, DataType colDataType, int colIndex, PipelineBuffer buffer, ColumnBufferMapping cbm)
        {
            var n = colIndex;
            if (buffer.IsNull(n)) return null;
            switch (colDataType)
            {
                case DataType.DT_BOOL:
                    return buffer.GetBoolean(n);
                case DataType.DT_I4:
                    return buffer.GetInt32(n);
                case DataType.DT_I2:
                    return buffer.GetInt16(n);
                case DataType.DT_I8:
                    return buffer.GetInt64(n);
                case DataType.DT_DATE:
                    return buffer.GetDate(n);
                case DataType.DT_STR:
                case DataType.DT_WSTR:
                case DataType.DT_NTEXT:
                case DataType.DT_TEXT:
                    return buffer.GetString(n);
            }
            bool cancel = true;
            ComponentMetaData.FireError(123, "GetObject", string.Format("GetObject(colName={0}, colDataType={1}) is not yet supported", colName, colDataType), "", 0, out cancel);
            return null;
        }

        private MultipleValueDictionary<string, object[]> AppendsByCommonFieldHash = new MultipleValueDictionary<string, object[]>();
        private bool InputComparisonProcessed = false;
        private bool InputRootProcessed = false;

        private void ProcessInputComparison(IDTSInput100 input, PipelineBuffer buffer)
        {
            var commonFingerprints = GetComparisonColumnKeys();
            var fingerprinter = new Fingerprinter();
            var appends = new List<object>();
            while (buffer.NextRow())
            {
                for (int z = 0; z < input.InputColumnCollection.Count; ++z)
                {
                    var col = input.InputColumnCollection[z];
                    var colFingerprint = CreateColumnFingerprint(col);
                    var o = GetObject(col.Name, col.DataType, z, buffer, InputComparisonBufferColumnIndicees);
                    if (commonFingerprints.Contains(colFingerprint))
                    {
                        fingerprinter.AppendValue(o);
                    }
                    else
                    {
                        appends.Add(o);
                    }
                }
                var fingerprint = fingerprinter.GetFingerPrint();
                AppendsByCommonFieldHash.Add(fingerprint, appends.ToArray());
                fingerprinter.Clear();
                appends.Clear();
            }
            InputComparisonProcessed = buffer.EndOfRowset;
        }

#region Helpers

        private static string CreateColumnFingerprint(IDTSInputColumn100 col)
        {
            string def;
            switch (col.DataType)
            {
                case DataType.DT_CY:
                case DataType.DT_DECIMAL:
                case DataType.DT_NUMERIC:
                case DataType.DT_R4:
                case DataType.DT_R8:
                    def = "decimal";
                    break;
                case DataType.DT_UI1:
                case DataType.DT_UI2:
                case DataType.DT_UI4:
                case DataType.DT_UI8:
                case DataType.DT_I1:
                case DataType.DT_I2:
                case DataType.DT_I4:
                case DataType.DT_I8:
                    def = "num";
                    break;
                case DataType.DT_STR:
                case DataType.DT_WSTR:
                case DataType.DT_NTEXT:
                case DataType.DT_TEXT:
                    def = "string";
                    break;
                default:
                    def = col.DataType.ToString();
                    break;
            }
            def = col.Name + ";" + def;
            return def.ToLower();
        }

        private static void CopyColumnDefinition(IDTSOutputColumnCollection100 output, IDTSInputColumn100 inCol)
        {
            var outCol = output.New();
            outCol.Name = inCol.Name;
            outCol.SetDataTypeProperties(inCol.DataType, inCol.Length, inCol.Precision, inCol.Scale, inCol.CodePage);
        }

        private static void AddOutputColumns(IDTSInputColumnCollection100 root, IDTSOutputColumnCollection100 output)
        {
            for (int z = 0; z < root.Count; ++z)
            {
                var inCol = root[z];
                CopyColumnDefinition(output, inCol);
            }
        }
#endregion
    }
}
