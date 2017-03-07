using Newtonsoft.Json;
using RSD = RevolutionaryStuff.Core.Data;
using System;
using System.Collections.Generic;

/*
This file was created by:
   1. sniffing the JSON using Fiddler
   2. edit, paste special, paste as JSON classes
   3. "combining" similar classes
   4. adding assorted JsonPropertyAttribute s
   5. renaming classes for capitalization 
*/
namespace Traffk.Tableau.VQL
{
    public class VqBase
    {
        public string ToJson() => JsonConvert.SerializeObject(this);
    }

    public class VqlCmdResponseWrapper : VqBase
    {
        public VqlCmdResponse vqlCmdResponse { get; set; }
    }

    public class VqlCmdResponse : VqBase
    {
        public LayoutStatus layoutStatus { get; set; }
        public CommandResultList[] cmdResultList { get; set; }
    }

    public class LayoutStatus : VqBase
    {
        public string active_tab { get; set; }
        public bool isViewModified { get; set; }
        public int undoPosition { get; set; }
        public object[] urlActionList { get; set; }
        public Vizstatelist[] vizStateList { get; set; }
        public object[] legacyMenus { get; set; }
        public ApplicationPresModel applicationPresModel { get; set; }
        public bool isWorldNew { get; set; }
    }

    public class ApplicationPresModel : VqBase
    {
        public string renderMode { get; set; }
        public WorkbookPresModel workbookPresModel { get; set; }
        public string dateFormat { get; set; }
        public string timeFormat { get; set; }
        public ToolbarPresModel toolbarPresModel { get; set; }
        public DataDictionary dataDictionary { get; set; }
    }

    public class WorkbookPresModel : VqBase
    {
        public DashboardPresModel dashboardPresModel { get; set; }
        public SheetsInfo[] sheetsInfo { get; set; }
        public bool workbookModified { get; set; }
    }

    public class DashboardPresModel : VqBase
    {
        public SheetPath sheetPath { get; set; }
        public Zones zones { get; set; }
        public int activeZoneId { get; set; }
        public object[] userActions { get; set; }
        public CssAttrs cssAttrs { get; set; }
        public bool autoUpdate { get; set; }
        public bool hasSelection { get; set; }
        public Viewportsize viewportSize { get; set; }
        public VisualId[] visualIds { get; set; }
        public object[] invalidSheets { get; set; }
        public Viewids viewIds { get; set; }
        public SheetLayoutInfo sheetLayoutInfo { get; set; }
        public object[] modifiedSheets { get; set; }
    }

    public class SheetPath : VqBase
    {
        public string sheetName { get; set; }
        public bool isDashboard { get; set; }
    }

    public class Zones : VqBase
    {
        [JsonProperty("_1")]
        public Zone Zone1 { get; set; }

        [JsonProperty("_2")]
        public Zone Zone2 { get; set; }

        [JsonProperty("_3")]
        public Zone Zone3 { get; set; }

        [JsonProperty("_4")]
        public Zone Zone4 { get; set; }

        [JsonProperty("_5")]
        public Zone Zone5 { get; set; }

        [JsonProperty("_6")]
        public Zone Zone6 { get; set; }
    }

    public class PresModelHolder : VqBase
    {
        public QuantitativeColorLegend quantitativeColorLegend { get; set; }
        public Visual visual { get; set; }
    }

    public class Visual : VqBase
    {
        public Scene scene { get; set; }
        public MarkLabels markLabels { get; set; }
        public SelectionList[] selectionList { get; set; }
        public SelectionList[] brushingSelectionList { get; set; }
        public VizData vizData { get; set; }
        public VisualTitle visualTitle { get; set; }
        public string geometryJson { get; set; }
        public string cacheUrlInfoJson { get; set; }
        public SortIndicators sortIndicators { get; set; }
        public bool isMap { get; set; }
        public bool hasBackgroundImage { get; set; }
        public string floatingToolbarVisibility { get; set; }
        public string geographicSearchVisibility { get; set; }
        public string vizNavigationSetting { get; set; }
        public string defaultMapToolEnum { get; set; }
        public bool hasModifiedAxes { get; set; }
        public ImageDictionary imageDictionary { get; set; }
        public ColorDictionary colorDictionary { get; set; }
        public bool valid { get; set; }
        public VisualId visualIdPresModel { get; set; }
        public string tooltipMode { get; set; }
        public string bgColor { get; set; }
        public string paneColor { get; set; }
        public string headerColor { get; set; }
        public bool emptyHighlightFogAll { get; set; }
        public int zoomLevel { get; set; }
    }

    public class Scene : VqBase
    {
        public Pdmarksmap pdMarksMap { get; set; }
        public object[] markShapeList { get; set; }
        public string bgColor { get; set; }
        public bool isOpaque { get; set; }
        public DrawPart[] drawFirst { get; set; }
        public Pane[] panes { get; set; }
        public DrawPart[] drawLast { get; set; }
        public string highlightTextColor { get; set; }
        public string selectionTextColor { get; set; }
        public string highlightBgColor { get; set; }
        public string selectBgColor { get; set; }
    }

    public class Pdmarksmap : VqBase
    {
        public _8Ff7bcd0322ddc22 _8ff7bcd0322ddc22 { get; set; }
    }

    public class _8Ff7bcd0322ddc22 : VqBase
    {
        public string markLayoutPrimitive { get; set; }
        public string markLayoutVizType { get; set; }
        public Boolmap boolMap { get; set; }
        public Intmap intMap { get; set; }
        public Floatmap floatMap { get; set; }
        public EncodingColumns encodingColumns { get; set; }
        public object[] trailOverride { get; set; }
    }

    public class Boolmap : VqBase
    {
        public bool draw_border { get; set; }
        public bool has_empty_or_equal_axis { get; set; }
        public bool has_folded_axis { get; set; }
        public bool has_halos { get; set; }
        public bool is_log_x { get; set; }
        public bool is_log_y { get; set; }
        public bool is_mark_stacked { get; set; }
        public bool mark_trail_auto_color { get; set; }
        public bool mark_trail_transparency { get; set; }
        public bool pane_has_background { get; set; }
        public bool wrap_domain { get; set; }
    }

    public class Intmap : VqBase
    {
        public int alpha { get; set; }
        public int background_color { get; set; }
        public int border_color { get; set; }
        public int cell_x { get; set; }
        public int cell_y { get; set; }
        public int drop_line_flags { get; set; }
        public int halo_color { get; set; }
        public int handle_specials_x { get; set; }
        public int handle_specials_y { get; set; }
        public int highlighted_halo_color { get; set; }
        public int mark_flags { get; set; }
        public int mark_label_padding { get; set; }
        public int mark_trail_color { get; set; }
        public int mark_trail_count { get; set; }
        public int min_bar_height { get; set; }
        public int min_bar_width { get; set; }
        public int page_trail_marks_to_trail { get; set; }
        public int pane_flags { get; set; }
        public int pane_page { get; set; }
        public int selected_halo_color { get; set; }
        public int selected_multi_prim_halo_color { get; set; }
        public int trail_id { get; set; }
    }

    public class Floatmap : VqBase
    {
        public int clip_height { get; set; }
        public int clip_width { get; set; }
        public float mark_trail_end_effect { get; set; }
        public float mark_trail_start_effect { get; set; }
        public float size { get; set; }
        public int x1 { get; set; }
    }

    public class EncodingColumns : VqBase
    {
        public LongValues color { get; set; }
        public IntValues tuple_id { get; set; }
        public IntValues x_int { get; set; }
        public IntValues y1_int { get; set; }
        public IntValues y_int { get; set; }
    }

    public class LongValues : VqBase
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonIgnore]
        public Type ClrType => UnderlyingDataTable.TableauDataTypeToClrType(DataType);

        [JsonProperty("dataValues")]
        public long[] DataValues { get; set; }
    }

    public class IntValues : VqBase
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonIgnore]
        public Type ClrType => UnderlyingDataTable.TableauDataTypeToClrType(DataType);

        [JsonProperty("dataValues")]
        public int[] DataValues { get; set; }
    }

    public class DrawItem : VqBase
    {
        public string type { get; set; }
        public string color { get; set; }
        public int[] points { get; set; }
        public string fontName { get; set; }
        public string lineCap { get; set; }
        public int pointSize { get; set; }
        public int angle { get; set; }
        public bool isBold { get; set; }
        public bool isItalics { get; set; }
        public bool isUnderlined { get; set; }
        public bool isStrikeThrough { get; set; }
        public string horizontalLabelAlignment { get; set; }
        public string verticalLabelAlignment { get; set; }
        public int[] transformOffset { get; set; }
        public object[] transformScale { get; set; }
        public string text { get; set; }
        public bool state { get; set; }
        public int extentsWidth { get; set; }
        public int extentsHeight { get; set; }
        public int[] columnIndices { get; set; }
        public int[] aliasIndices { get; set; }
        public DrawItem[] drawItems { get; set; }
        public int extentsX { get; set; }
    }

    public class Pane : VqBase
    {
        public int rowIndex { get; set; }
        public int columnIndex { get; set; }
        public Rectangle paneRect { get; set; }
        public DrawPart[] drawPane { get; set; }
        public object[] renderNodes { get; set; }
    }

    public class DrawPart : VqBase
    {
        public string visualPart { get; set; }
        public DrawItem[] drawItems { get; set; }
        public PaneMarks paneMarks { get; set; }
    }

    public class PaneMarks : VqBase
    {
        public string paneDescrKey { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public int[] pageIndices { get; set; }
        public Rectangle pixelExtents { get; set; }
        public DomainExtents domainExtents { get; set; }
        public DomainOrigin domainOrigin { get; set; }
        public PaneId paneId { get; set; }
        public int paneIndex { get; set; }
    }

    public class DomainExtents : VqBase
    {
        public int doubleLeft { get; set; }
        public float doubleTop { get; set; }
        public int width { get; set; }
        public float height { get; set; }
    }

    public class DomainOrigin : VqBase
    {
        public int floatX { get; set; }
        public float floatY { get; set; }
    }

    public class PaneId : VqBase
    {
        public int rowIndex { get; set; }
        public int columnIndex { get; set; }
    }

    public class MarkLabels : VqBase
    {
        public object[] markShapeList { get; set; }
        public string bgColor { get; set; }
        public bool isOpaque { get; set; }
        public object[] drawFirst { get; set; }
        public object[] panes { get; set; }
        public object[] drawLast { get; set; }
        public string highlightTextColor { get; set; }
        public string selectionTextColor { get; set; }
        public string highlightBgColor { get; set; }
        public string selectBgColor { get; set; }
    }

    public class VizData : VqBase
    {
        public object[] vizColumns { get; set; }
        public PaneColumnsData paneColumnsData { get; set; }
        public object[] highlightCaptions { get; set; }
        public object[] filterFields { get; set; }
        public Ubertipdata ubertipData { get; set; }
        public object[] reflineFields { get; set; }
        public object[] refLineTips { get; set; }
    }

    public class PaneColumnsData : VqBase
    {
        public VizDataColumn[] vizDataColumns { get; set; }
        public PaneColumnsList[] paneColumnsList { get; set; }
    }

    public class VizDataColumn : VqBase
    {
        public string fn { get; set; }
        public string fnDisagg { get; set; }
        public string[] formatStrings { get; set; }
        public bool isAutoSelect { get; set; }
        public int[] paneIndices { get; set; }
        public int[] columnIndices { get; set; }
        public string localBaseColumnName { get; set; }
        public string baseColumnName { get; set; }

        [JsonProperty("fieldCaption")]
        public string FieldCaption { get; set; }

        public string datasourceCaption { get; set; }

        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonIgnore]
        public Type ClrType => UnderlyingDataTable.TableauDataTypeToClrType(DataType);
        public string aggregation { get; set; }
        public string fieldRole { get; set; }
        public StringCollation stringCollation { get; set; }
    }

    public class StringCollation : VqBase
    {
        public string name { get; set; }
        public int charsetId { get; set; }
    }

    public class PaneColumnsList : VqBase
    {
        public PaneDescriptor paneDescriptor { get; set; }
        public VizPaneColumn[] vizPaneColumns { get; set; }
    }

    public class PaneDescriptor : VqBase
    {
        public string paneDescrKey { get; set; }
        public string[] xFields { get; set; }
        public string[] yFields { get; set; }
        public int xIndex { get; set; }
        public int yIndex { get; set; }
    }

    public class VizPaneColumn : VqBase
    {
        public int?[] tupleIds { get; set; }
        public int?[] valueIndices { get; set; }
        public int?[] aliasIndices { get; set; }
        public int?[] formatstrIndices { get; set; }
    }

    public class Ubertipdata : VqBase
    {
        public Ubertippanedata[] ubertipPaneDatas { get; set; }
        public StandardCommands standardCommands { get; set; }
        public MultiSelectCommands multiselectCommands { get; set; }
    }

    public class StandardCommands : VqBase
    {
        public CommandItem[] commandItems { get; set; }
    }

    public class CommandItem : VqBase
    {
        public string commandsType { get; set; }
        public string name { get; set; }
        public string iconRes { get; set; }
        public bool state { get; set; }
        public string command { get; set; }
        public string description { get; set; }
        public Commands commands { get; set; }
    }

    public class MultiSelectCommands : VqBase
    {
        public CommandItem[] commandItems { get; set; }
    }

    public class Ubertippanedata : VqBase
    {
        public string htmlTooltip { get; set; }
        public bool showButtons { get; set; }
        public string summaryField { get; set; }
    }

    public class VisualTitle : Rectangle
    {
        public string caption { get; set; }
        public string html { get; set; }
        public string halign { get; set; }
        public string valign { get; set; }
        public int orientation { get; set; }
        public StyledBox styledBox { get; set; }
    }

    public class SortIndicators : VqBase
    {
        public SortIndicatorRegion[] sortIndicatorRegions { get; set; }
    }

    public class SortIndicatorRegion : VqBase
    {
        public string sortRegion { get; set; }
        public Rectangle regionRect { get; set; }
        public bool canScrollX { get; set; }
        public bool canScrollY { get; set; }
        public bool isHorizontal { get; set; }
        public string defSortOrder { get; set; }
        public SortIndicatorItem[] sortIndicatorItems { get; set; }
    }

    public class SortIndicatorItem : VqBase
    {
        public Rectangle itemRect { get; set; }
        public Rectangle buttonRect { get; set; }
        public bool isSorted { get; set; }
        public string sortOrder { get; set; }
        public string tooltipText { get; set; }
    }

    public class ImageDictionary : VqBase
    {
    }

    public class ColorDictionary : VqBase
    {
        public object[] colorList { get; set; }
    }

    public class SelectionList : VqBase
    {
        public string selectionType { get; set; }
        public object[] objectIds { get; set; }
        public object[] selectedNodes { get; set; }
        public object[] orientedNodeReferences { get; set; }
    }

    public class Zone : Rectangle
    {
        public int zoneId { get; set; }
        public int zoneZOrder { get; set; }
        public string zoneType { get; set; }
        public string worksheet { get; set; }
        public PresModelHolder presModelHolder { get; set; }
        public bool hasTitle { get; set; }
        public bool hasCaption { get; set; }
        public bool isVisible { get; set; }
        public string sheet { get; set; }
        public string bgColor { get; set; }
        public StyledBox styledBox { get; set; }
        public int titleHeight { get; set; }
    }

    public class QuantitativeColorLegend : VqBase
    {
        public StyledBox styledBox { get; set; }
        public string titleHtml { get; set; }
        public bool isTitleVisible { get; set; }
        public string cursorShape { get; set; }
        public string maxLabel { get; set; }
        public string minLabel { get; set; }
        public LabelTextStyle labelTextStyle { get; set; }
        public StyledBox rampBorderStyledBox { get; set; }
        public QuantitativeLegendLayout quantitativeLegendLayout { get; set; }
        public bool isColorStepped { get; set; }
        public bool isColorDiverging { get; set; }
        public string[] rampColorSamples { get; set; }
        public int colorTransparencyValue { get; set; }
    }

    public class LabelTextStyle : VqBase
    {
        public string fontName { get; set; }
        public int fontSize { get; set; }
        public string color { get; set; }
        public string halign { get; set; }
        public string valign { get; set; }
        public string wrapMode { get; set; }
    }

    public class StyledBox : VqBase
    {
        public int uw { get; set; }
        public string borderStyle { get; set; }
        public string borderColor { get; set; }
        public string fillColor { get; set; }
        public bool hasFill { get; set; }
        public bool isInner { get; set; }
    }

    public class QuantitativeLegendLayout : VqBase
    {
        public bool titleInline { get; set; }
        public bool isSingleLabel { get; set; }
        public Rectangle rectTitle { get; set; }
        public Rectangle rectRamp { get; set; }
        public Rectangle rectMinLabel { get; set; }
        public Rectangle rectMaxLabel { get; set; }
        public Position centerMarkLineBegin { get; set; }
        public Position centerMarkLineEnd { get; set; }
    }

    public class Position : VqBase
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }
    }

    public class Rectangle : Position
    {
        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }
    }


    public class CssAttrs : VqBase
    {
        public string backgroundColor { get; set; }
        public string color { get; set; }
    }

    public class Viewportsize : VqBase
    {
        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("h")]
        public int Height { get; set; }
    }

    public class Viewids : VqBase
    {
        public string AvgRiskbyUrbanicity { get; set; }
    }

    public class SheetLayoutInfo : VqBase
    {
        public string layoutId { get; set; }
        public string shareLink { get; set; }
        public string blogLink { get; set; }
        public string staticImage { get; set; }
        public string baseViewThumbLink { get; set; }
        public string downloadLink { get; set; }
        public string embeddedTitle { get; set; }
        public string repositoryUrl { get; set; }
        public string sheetId { get; set; }
        public string sheetName { get; set; }
        public string sheetType { get; set; }
        public string viewId { get; set; }
        public int currentCustomViewId { get; set; }
        public bool isCurrentCustViewIdValid { get; set; }
        public string guid { get; set; }
    }

    public class VisualId : VqBase
    {
        public string worksheet { get; set; }
        public string dashboard { get; set; }
    }

    public class SheetsInfo : VqBase
    {
        public string sheet { get; set; }
        public bool isDashboard { get; set; }
        public bool isVisible { get; set; }
        public bool isPublished { get; set; }
        public string tabColor { get; set; }
        public string[] namesOfSubsheets { get; set; }
        public int minWidth { get; set; }
        public int minHeight { get; set; }
        public int maxWidth { get; set; }
        public int maxHeight { get; set; }
    }

    public class ToolbarPresModel : VqBase
    {
        public LegacyMenu[] legacyMenus { get; set; }
        public Commands vizCommands { get; set; }
        public Commands nonVizCommands { get; set; }
    }

    public class Commands : VqBase
    {
        public CommandItem[] commandItems { get; set; }
    }

    public class LegacyMenu : VqBase
    {
        public string legacyMenuName { get; set; }
        public string[] legacyMenuStates { get; set; }
    }

    public class DataSegments : VqBase
    {
        [JsonProperty("0")]
        public _0 _0 { get; set; }
    }

    public class _0 : VqBase
    {
        public DataColumn[] dataColumns { get; set; }
    }

    public class Vizstatelist : VqBase
    {
        public string sheet { get; set; }
        public Vizregionrectlist[] vizRegionRectList { get; set; }
    }

    public class Vizregionrectlist : Rectangle
    {
        public string r { get; set; }
    }

    public class CommandResultList : VqBase
    {
        public string commandName { get; set; }
        public CommandReturn commandReturn { get; set; }
    }

    public class CommandReturn : VqBase
    {
        public UnderlyingDataTable underlyingDataTable { get; set; }
    }

    public class UnderlyingDataTable : VqBase
    {
        public string tableAlias { get; set; }
        public string tableName { get; set; }
        public int numRows { get; set; }
        public DataDictionary dataDictionary { get; set; }
        public UnderlyingDataTableColumn[] underlyingDataTableColumns { get; set; }

        public RSD.IDataTable ToDataTable()
        {
            var dt = new RSD.SimpleDataTable
            {
                TableName = this.tableName
            };
            var segment = this.dataDictionary.dataSegments._0;
            var dataColumnByUdc = new Dictionary<UnderlyingDataTableColumn, DataColumn>();
            foreach (var udc in this.underlyingDataTableColumns)
            {
                dt.Columns.Add(udc.ToDataColumn());
                foreach (var dc in segment.dataColumns)
                {
                    if (dc.DataType == udc.DataType)
                    {
                        dataColumnByUdc[udc] = dc;
                    }
                }
            }
            int rowCount = underlyingDataTableColumns[0].valueIndices.Length;
            for (int y = 0; y < rowCount; ++y)
            {
                var vals = new object[dt.Columns.Count];
                for (int x = 0; x < vals.Length; ++x)
                {
                    var udc = underlyingDataTableColumns[x];
                    var valIndex = udc.valueIndices[y];
                    var dc = dataColumnByUdc[udc];
                    var oval = dc.DataValues[valIndex];
                    vals[x] = Convert.ChangeType(oval, dc.ClrType);
                }
                dt.Rows.Add(vals);
            }
            return dt;
        }

        public static Type TableauDataTypeToClrType(string dataType)
        {
            switch (dataType)
            {
                case "integer":
                    return typeof(int);
                case "real":
                    return typeof(double);
                case "cstring":
                    return typeof(string);
                default:
                    return typeof(object);
            }
        }
    }

    public class DataDictionary : VqBase
    {
        public DataSegments dataSegments { get; set; }
    }

    public class DataColumn : VqBase
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonIgnore]
        public Type ClrType => UnderlyingDataTable.TableauDataTypeToClrType(DataType);

        [JsonProperty("dataValues")]
        public object[] DataValues { get; set; }
    }

    public class UnderlyingDataTableColumn : VqBase
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonIgnore]
        public Type ClrType => UnderlyingDataTable.TableauDataTypeToClrType(DataType);

        [JsonProperty("fieldCaption")]
        public string FieldCaption { get; set; }

        public string fn { get; set; }
        public bool isReferenced { get; set; }
        public int[] valueIndices { get; set; }
        public int[] formatValIdxs { get; set; }
        public int[] aliasIndices { get; set; }

        public RSD.IDataColumn ToDataColumn()
        {
            return new RSD.SimpleDataColumn
            {
                ColumnName = FieldCaption,
                DataType = UnderlyingDataTable.TableauDataTypeToClrType(DataType)
            };
        }
    }
}
