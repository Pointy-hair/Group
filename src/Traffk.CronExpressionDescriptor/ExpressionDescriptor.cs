using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Traffk.CronExpressionDescriptor
{
    public class ExpressionDescriptor
    {
        private static string DefaultLocale;
        public static void SetDefaultLocale(string locale)
        {
            DefaultLocale = locale;
        }

        private readonly char[] SpecialCharacters = new char[] { '/', '-', ',', '*' };
        private readonly string[] TwentyFourHourTimeFormatLocales = new string[] { "ru-RU", "uk-UA", "de-DE", "it-IT", "tr-TR", "pl-PL" };

        private string Expression;
        private Options Options;
        private string[] ExpressionParts;
        private bool IsParsed;
        private bool Use24HourTimeFormat;
        private CultureInfo Culture;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionDescriptor"/> class
        /// </summary>
        /// <param name="expression">The cron expression string</param>
        public ExpressionDescriptor(string expression) : this(expression, new Options()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionDescriptor"/> class
        /// </summary>
        /// <param name="expression">The cron expression string</param>
        /// <param name="options">Options to control the output description</param>
        public ExpressionDescriptor(string expression, Options options)
        {
            Expression = expression;
            Options = options;
            ExpressionParts = new string[7];
            IsParsed = false;

            var locale = options.Locale ?? DefaultLocale ?? "en";
            Culture = new CultureInfo(locale);

            if (Options.Use24HourTimeFormat != null)
            {
                // 24HourTimeFormat specified in options so use it
                Use24HourTimeFormat = Options.Use24HourTimeFormat.Value;
            }
            else
            {
                // 24HourTimeFormat not specified, default based on m_24hourTimeFormatLocales
                Use24HourTimeFormat = TwentyFourHourTimeFormatLocales.Any(x => x == locale);
            }
        }

        /// <summary>
        /// Generates a human readable string for the Cron Expression
        /// </summary>
        /// <param name="type">Which part(s) of the expression to describe</param>
        /// <returns>The cron expression description</returns>
        public string GetDescription(DescriptionTypeEnum type)
        {
            string description = string.Empty;

            try
            {
                if (!IsParsed)
                {
                    ExpressionParser parser = new ExpressionParser(Expression, Options);
                    ExpressionParts = parser.Parse();
                    IsParsed = true;
                }

                switch (type)
                {
                    case DescriptionTypeEnum.FULL:
                        description = GetFullDescription();
                        break;
                    case DescriptionTypeEnum.TIMEOFDAY:
                        description = GetTimeOfDayDescription();
                        break;
                    case DescriptionTypeEnum.HOURS:
                        description = GetHoursDescription();
                        break;
                    case DescriptionTypeEnum.MINUTES:
                        description = GetMinutesDescription();
                        break;
                    case DescriptionTypeEnum.SECONDS:
                        description = GetSecondsDescription();
                        break;
                    case DescriptionTypeEnum.DAYOFMONTH:
                        description = GetDayOfMonthDescription();
                        break;
                    case DescriptionTypeEnum.MONTH:
                        description = GetMonthDescription();
                        break;
                    case DescriptionTypeEnum.DAYOFWEEK:
                        description = GetDayOfWeekDescription();
                        break;
                    case DescriptionTypeEnum.YEAR:
                        description = GetYearDescription();
                        break;
                    default:
                        description = GetSecondsDescription();
                        break;
                }
            }
            catch (Exception ex)
            {
                if (!Options.ThrowExceptionOnParseError)
                {
                    description = ex.Message;
                }
                else
                {
                    throw;
                }
            }

            // Uppercase the first letter
            description = string.Concat(Culture.TextInfo.ToUpper(description[0]), description.Substring(1));

            return description;
        }

        /// <summary>
        /// Generates the FULL description
        /// </summary>
        /// <returns>The FULL description</returns>
        protected string GetFullDescription()
        {
            string description;

            try
            {
                string timeSegment = GetTimeOfDayDescription();
                string dayOfMonthDesc = GetDayOfMonthDescription();
                string monthDesc = GetMonthDescription();
                string dayOfWeekDesc = GetDayOfWeekDescription();
                string yearDesc = GetYearDescription();

                description = string.Format("{0}{1}{2}{3}{4}",
                    timeSegment,
                    dayOfMonthDesc,
                    dayOfWeekDesc,
                    monthDesc,
                    yearDesc);

                description = TransformVerbosity(description, Options.Verbose);
            }
            catch (Exception ex)
            {
                description = Resources.ResourceManager.GetString("AnErrorOccuredWhenGeneratingTheExpressionD", Culture);
                if (Options.ThrowExceptionOnParseError)
                {
                    throw new FormatException(description, ex);
                }
            }


            return description;
        }

        /// <summary>
        /// Generates a description for only the TIMEOFDAY portion of the expression
        /// </summary>
        /// <returns>The TIMEOFDAY description</returns>
        protected string GetTimeOfDayDescription()
        {
            string secondsExpression = ExpressionParts[0];
            string minuteExpression = ExpressionParts[1];
            string hourExpression = ExpressionParts[2];

            StringBuilder description = new StringBuilder();

            //handle special cases first
            if (minuteExpression.IndexOfAny(SpecialCharacters) == -1
                && hourExpression.IndexOfAny(SpecialCharacters) == -1
                && secondsExpression.IndexOfAny(SpecialCharacters) == -1)
            {
                //specific time of day (i.e. 10 14)
                description.Append(Resources.ResourceManager.GetString("AtSpace", Culture)).Append(FormatTime(hourExpression, minuteExpression, secondsExpression));
            }
            else if (minuteExpression.Contains("-")
                     && !minuteExpression.Contains(",")
                     && hourExpression.IndexOfAny(SpecialCharacters) == -1)
            {
                //minute range in single hour (i.e. 0-10 11)
                string[] minuteParts = minuteExpression.Split('-');
                description.Append(string.Format(Resources.ResourceManager.GetString("EveryMinuteBetweenX0AndX1", Culture),
                    FormatTime(hourExpression, minuteParts[0]),
                    FormatTime(hourExpression, minuteParts[1])));
            }
            else if (hourExpression.Contains(",") && minuteExpression.IndexOfAny(SpecialCharacters) == -1)
            {
                //hours list with single minute (o.e. 30 6,14,16)
                string[] hourParts = hourExpression.Split(',');
                description.Append(Resources.ResourceManager.GetString("At", Culture));
                for (int i = 0; i < hourParts.Length; i++)
                {
                    description.Append(" ").Append(FormatTime(hourParts[i], minuteExpression));

                    if (i < (hourParts.Length - 2))
                    {
                        description.Append(",");
                    }

                    if (i == hourParts.Length - 2)
                    {
                        description.Append(Resources.ResourceManager.GetString("SpaceAnd", Culture));
                    }
                }
            }
            else
            {
                //default time description
                string secondsDescription = GetSecondsDescription();
                string minutesDescription = GetMinutesDescription();
                string hoursDescription = GetHoursDescription();

                description.Append(secondsDescription);

                if (description.Length > 0)
                {
                    description.Append(", ");
                }

                description.Append(minutesDescription);

                if (description.Length > 0)
                {
                    description.Append(", ");
                }

                description.Append(hoursDescription);
            }


            return description.ToString();
        }

        /// <summary>
        /// Generates a description for only the SECONDS portion of the expression
        /// </summary>
        /// <returns>The SECONDS description</returns>
        protected string GetSecondsDescription()
        {
            string description = GetSegmentDescription(ExpressionParts[0],
                Resources.ResourceManager.GetString("EverySecond", Culture),
                (s => s),
                (s => string.Format(Resources.ResourceManager.GetString("EveryX0Seconds", Culture), s)),
                (s => Resources.ResourceManager.GetString("SecondsX0ThroughX1PastTheMinute", Culture)),
                (s => s == "0"
                    ? string.Empty
                    : (int.Parse(s) < 20)
                        ? Resources.ResourceManager.GetString("AtX0SecondsPastTheMinute", Culture)
                        : Resources.ResourceManager.GetString("AtX0SecondsPastTheMinuteGt20", Culture) ?? Resources.ResourceManager.GetString("AtX0SecondsPastTheMinute", Culture)
                ));

            return description;
        }

        /// <summary>
        /// Generates a description for only the MINUTE portion of the expression
        /// </summary>
        /// <returns>The MINUTE description</returns>
        protected string GetMinutesDescription()
        {
            string description = GetSegmentDescription(ExpressionParts[1],
                Resources.ResourceManager.GetString("EveryMinute", Culture),
                (s => s),
                (s => string.Format(Resources.ResourceManager.GetString("EveryX0Minutes", Culture), s)),
                (s => Resources.ResourceManager.GetString("MinutesX0ThroughX1PastTheHour", Culture)),
                (s =>
                {
                    try
                    {
                        return s == "0"
                            ? string.Empty
                            : (int.Parse(s) < 20)
                                ? Resources.ResourceManager.GetString("AtX0MinutesPastTheHour", Culture)
                                : Resources.ResourceManager.GetString("AtX0MinutesPastTheHourGt20", Culture) ?? Resources.ResourceManager.GetString("AtX0MinutesPastTheHour", Culture);
                    }
                    catch { return Resources.ResourceManager.GetString("AtX0MinutesPastTheHour", Culture); }
                }));

            return description;
        }

        /// <summary>
        /// Generates a description for only the HOUR portion of the expression 
        /// </summary>
        /// <returns>The HOUR description</returns>
        protected string GetHoursDescription()
        {
            string expression = ExpressionParts[2];
            string description = GetSegmentDescription(expression,
                Resources.ResourceManager.GetString("EveryHour", Culture),
                (s => FormatTime(s, "0")),
                (s => string.Format(Resources.ResourceManager.GetString("EveryX0Hours", Culture), s)),
                (s => Resources.ResourceManager.GetString("BetweenX0AndX1", Culture)),
                (s => Resources.ResourceManager.GetString("AtX0", Culture)));

            return description;
        }

        /// <summary>
        /// Generates a description for only the DAYOFWEEK portion of the expression 
        /// </summary>
        /// <returns>The DAYOFWEEK description</returns>
        protected string GetDayOfWeekDescription()
        {
            string description = GetSegmentDescription(ExpressionParts[5],
                Resources.ResourceManager.GetString("ComaEveryDay", Culture),
                (s =>
                {
                    string exp = s;
                    if (s.Contains("#"))
                    {
                        exp = s.Remove(s.IndexOf("#"));
                    }
                    else if (s.Contains("L"))
                    {
                        exp = exp.Replace("L", string.Empty);
                    }

                    return Culture.DateTimeFormat.GetDayName(((DayOfWeek)Convert.ToInt32(exp)));
                }),
                (s => string.Format(Resources.ResourceManager.GetString("ComaEveryX0DaysOfTheWeek", Culture), s)),
                (s => Resources.ResourceManager.GetString("ComaX0ThroughX1", Culture)),
                (s =>
                {
                    string format = null;
                    if (s.Contains("#"))
                    {
                        string dayOfWeekOfMonthNumber = s.Substring(s.IndexOf("#") + 1);
                        string dayOfWeekOfMonthDescription = null;
                        switch (dayOfWeekOfMonthNumber)
                        {
                            case "1":
                                dayOfWeekOfMonthDescription = Resources.ResourceManager.GetString("First", Culture);
                                break;
                            case "2":
                                dayOfWeekOfMonthDescription = Resources.ResourceManager.GetString("Second", Culture);
                                break;
                            case "3":
                                dayOfWeekOfMonthDescription = Resources.ResourceManager.GetString("Third", Culture);
                                break;
                            case "4":
                                dayOfWeekOfMonthDescription = Resources.ResourceManager.GetString("Forth", Culture);
                                break;
                            case "5":
                                dayOfWeekOfMonthDescription = Resources.ResourceManager.GetString("Fifth", Culture);
                                break;
                        }


                        format = string.Concat(Resources.ResourceManager.GetString("ComaOnThe", Culture),
                            dayOfWeekOfMonthDescription, Resources.ResourceManager.GetString("SpaceX0OfTheMonth", Culture));
                    }
                    else if (s.Contains("L"))
                    {
                        format = Resources.ResourceManager.GetString("ComaOnTheLastX0OfTheMonth", Culture);
                    }
                    else
                    {
                        format = Resources.ResourceManager.GetString("ComaOnlyOnX0", Culture);
                    }

                    return format;
                }));

            return description;
        }

        /// <summary>
        /// Generates a description for only the MONTH portion of the expression 
        /// </summary>
        /// <returns>The MONTH description</returns>
        protected string GetMonthDescription()
        {
            string description = GetSegmentDescription(ExpressionParts[4],
                string.Empty,
                (s => new DateTime(DateTime.Now.Year, Convert.ToInt32(s), 1).ToString("MMMM", Culture)),
                (s => string.Format(Resources.ResourceManager.GetString("ComaEveryX0Months", Culture), s)),
                (s => Resources.ResourceManager.GetString("ComaMonthX0ThroughMonthX1", Culture) ?? Resources.ResourceManager.GetString("ComaX0ThroughX1", Culture)),
                (s => Resources.ResourceManager.GetString("ComaOnlyInX0", Culture)));

            return description;
        }

        /// <summary>
        /// Generates a description for only the DAYOFMONTH portion of the expression 
        /// </summary>
        /// <returns>The DAYOFMONTH description</returns>
        protected string GetDayOfMonthDescription()
        {
            string description = null;
            string expression = ExpressionParts[3];

            switch (expression)
            {
                case "L":
                    description = Resources.ResourceManager.GetString("ComaOnTheLastDayOfTheMonth", Culture);
                    break;
                case "WL":
                case "LW":
                    description = Resources.ResourceManager.GetString("ComaOnTheLastWeekdayOfTheMonth", Culture);
                    break;
                default:
                    Regex regex = new Regex("(\\d{1,2}W)|(W\\d{1,2})");
                    if (regex.IsMatch(expression))
                    {
                        Match m = regex.Match(expression);
                        int dayNumber = Int32.Parse(m.Value.Replace("W", ""));

                        string dayString = dayNumber == 1 ? Resources.ResourceManager.GetString("FirstWeekday", Culture) :
                            String.Format(Resources.ResourceManager.GetString("WeekdayNearestDayX0", Culture), dayNumber);
                        description = String.Format(Resources.ResourceManager.GetString("ComaOnTheX0OfTheMonth", Culture), dayString);

                        break;
                    }
                    else
                    {
                        description = GetSegmentDescription(expression,
                            Resources.ResourceManager.GetString("ComaEveryDay", Culture),
                            (s => s),
                            (s => s == "1" ? Resources.ResourceManager.GetString("ComaEveryDay", Culture) :
                                Resources.ResourceManager.GetString("ComaEveryX0Days", Culture)),
                            (s => Resources.ResourceManager.GetString("ComaBetweenDayX0AndX1OfTheMonth", Culture)),
                            (s => Resources.ResourceManager.GetString("ComaOnDayX0OfTheMonth", Culture)));
                        break;
                    }
            }

            return description;
        }

        /// <summary>
        /// Generates a description for only the YEAR portion of the expression 
        /// </summary>
        /// <returns>The YEAR description</returns>
        private string GetYearDescription()
        {
            string description = GetSegmentDescription(ExpressionParts[6],
                string.Empty,
                (s => Regex.IsMatch(s, @"^\d+$") ?
                    new DateTime(Convert.ToInt32(s), 1, 1).ToString("yyyy") : s),
                (s => string.Format(Resources.ResourceManager.GetString("ComaEveryX0Years", Culture), s)),
                (s => Resources.ResourceManager.GetString("ComaYearX0ThroughYearX1", Culture) ?? Resources.ResourceManager.GetString("ComaX0ThroughX1", Culture)),
                (s => Resources.ResourceManager.GetString("ComaOnlyInX0", Culture)));

            return description;
        }

        /// <summary>
        /// Generates the segment description
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="allDescription"></param>
        /// <param name="getSingleItemDescription"></param>
        /// <param name="getIntervalDescriptionFormat"></param>
        /// <param name="getBetweenDescriptionFormat"></param>
        /// <param name="getDescriptionFormat"></param>
        /// <returns></returns>
        protected string GetSegmentDescription(string expression,
            string allDescription,
            Func<string, string> getSingleItemDescription,
            Func<string, string> getIntervalDescriptionFormat,
            Func<string, string> getBetweenDescriptionFormat,
            Func<string, string> getDescriptionFormat
        )
        {
            string description = null;

            if (string.IsNullOrEmpty(expression))
            {
                description = string.Empty;
            }
            else if (expression == "*")
            {
                description = allDescription;
            }
            else if (expression.IndexOfAny(new char[] { '/', '-', ',' }) == -1)
            {
                description = string.Format(getDescriptionFormat(expression), getSingleItemDescription(expression));
            }
            else if (expression.Contains("/"))
            {
                string[] segments = expression.Split('/');
                description = string.Format(getIntervalDescriptionFormat(segments[1]), getSingleItemDescription(segments[1]));

                //interval contains 'between' piece (i.e. 2-59/3 )
                if (segments[0].Contains("-"))
                {
                    string betweenSegmentDescription = GenerateBetweenSegmentDescription(segments[0], getBetweenDescriptionFormat, getSingleItemDescription);

                    if (!betweenSegmentDescription.StartsWith(", "))
                    {
                        description += ", ";
                    }

                    description += betweenSegmentDescription;
                }
                else if (segments[0].IndexOfAny(new char[] { '*', ',' }) == -1)
                {
                    string rangeItemDescription = string.Format(getDescriptionFormat(segments[0]), getSingleItemDescription(segments[0]));
                    //remove any leading comma
                    rangeItemDescription = rangeItemDescription.Replace(", ", "");

                    description += string.Format(Resources.ResourceManager.GetString("CommaStartingX0", Culture), rangeItemDescription);
                }
            }
            else if (expression.Contains(","))
            {
                string[] segments = expression.Split(',');

                string descriptionContent = string.Empty;
                for (int i = 0; i < segments.Length; i++)
                {
                    if (i > 0 && segments.Length > 2)
                    {
                        descriptionContent += ",";

                        if (i < segments.Length - 1)
                        {
                            descriptionContent += " ";
                        }
                    }

                    if (i > 0 && segments.Length > 1 && (i == segments.Length - 1 || segments.Length == 2))
                    {
                        descriptionContent += Resources.ResourceManager.GetString("SpaceAndSpace", Culture);
                    }

                    if (segments[i].Contains("-"))
                    {
                        string betweenSegmentDescription = GenerateBetweenSegmentDescription(segments[i],
                            (s => Resources.ResourceManager.GetString("ComaX0ThroughX1", Culture)), getSingleItemDescription);

                        //remove any leading comma
                        betweenSegmentDescription = betweenSegmentDescription.Replace(", ", "");

                        descriptionContent += betweenSegmentDescription;
                    }
                    else
                    {
                        descriptionContent += getSingleItemDescription(segments[i]);
                    }
                }

                description = string.Format(getDescriptionFormat(expression), descriptionContent);
            }
            else if (expression.Contains("-"))
            {
                description = GenerateBetweenSegmentDescription(expression, getBetweenDescriptionFormat, getSingleItemDescription);
            }

            return description;
        }

        /// <summary>
        /// Generates the between segment description 
        /// </summary>
        /// <param name="betweenExpression"></param>
        /// <param name="getBetweenDescriptionFormat"></param>
        /// <param name="getSingleItemDescription"></param>
        /// <returns>The between segment description</returns>
        protected string GenerateBetweenSegmentDescription(string betweenExpression, Func<string, string> getBetweenDescriptionFormat, Func<string, string> getSingleItemDescription)
        {
            string description = string.Empty;
            string[] betweenSegments = betweenExpression.Split('-');
            string betweenSegment1Description = getSingleItemDescription(betweenSegments[0]);
            string betweenSegment2Description = getSingleItemDescription(betweenSegments[1]);
            betweenSegment2Description = betweenSegment2Description.Replace(":00", ":59");
            var betweenDescriptionFormat = getBetweenDescriptionFormat(betweenExpression);
            description += string.Format(betweenDescriptionFormat, betweenSegment1Description, betweenSegment2Description);

            return description;
        }

        /// <summary>
        /// Given time parts, will contruct a formatted time description 
        /// </summary>
        /// <param name="hourExpression">Hours part</param>
        /// <param name="minuteExpression">Minutes part</param>
        /// <returns>Formatted time description</returns>
        protected string FormatTime(string hourExpression, string minuteExpression)
        {
            return FormatTime(hourExpression, minuteExpression, string.Empty);
        }

        /// <summary>
        /// Given time parts, will contruct a formatted time description
        /// </summary>
        /// <param name="hourExpression">Hours part</param>
        /// <param name="minuteExpression">Minutes part</param>
        /// <param name="secondExpression">Seconds part</param>
        /// <returns>Formatted time description</returns>
        protected string FormatTime(string hourExpression, string minuteExpression, string secondExpression)
        {
            int hour = Convert.ToInt32(hourExpression);

            string period = string.Empty;
            if (!Use24HourTimeFormat)
            {
                period = Resources.ResourceManager.GetString((hour >= 12) ? "PMPeriod" : "AMPeriod", Culture);
                if (period.Length > 0)
                {
                    // add preceeding space
                    period = string.Concat(" ", period);
                }

                if (hour > 12)
                {
                    hour -= 12;
                }
            }

            string minute = Convert.ToInt32(minuteExpression).ToString();
            string second = string.Empty;
            if (!string.IsNullOrEmpty(secondExpression))
            {
                second = string.Concat(":", Convert.ToInt32(secondExpression).ToString().PadLeft(2, '0'));
            }

            return string.Format("{0}:{1}{2}{3}",
                hour.ToString().PadLeft(2, '0'), minute.PadLeft(2, '0'), second, period);
        }

        /// <summary>
        /// Transforms the verbosity of the expression description by stripping verbosity from original description
        /// </summary>
        /// <param name="description">The description to transform</param>
        /// <param name="isVerbose">If true, will leave description as it, if false, will strip verbose parts</param>
        /// <returns>The transformed description with proper verbosity</returns>
        protected string TransformVerbosity(string description, bool useVerboseFormat)
        {
            if (!useVerboseFormat)
            {
                description = description.Replace(Resources.ResourceManager.GetString("ComaEveryMinute", Culture), string.Empty);
                description = description.Replace(Resources.ResourceManager.GetString("ComaEveryHour", Culture), string.Empty);
                description = description.Replace(Resources.ResourceManager.GetString("ComaEveryDay", Culture), string.Empty);
            }

            return description;
        }

        #region Static
        /// <summary>
        /// Generates a human readable string for the Cron Expression 
        /// </summary>
        /// <param name="expression">The cron expression string</param>
        /// <returns>The cron expression description</returns>
        public static string GetDescription(string expression)
        {
            return GetDescription(expression, new Options());
        }

        /// <summary>
        /// Generates a human readable string for the Cron Expression  
        /// </summary>
        /// <param name="expression">The cron expression string</param>
        /// <param name="options">Options to control the output description</param>
        /// <returns>The cron expression description</returns>
        public static string GetDescription(string expression, Options options)
        {
            ExpressionDescriptor descripter = new ExpressionDescriptor(expression, options);
            return descripter.GetDescription(DescriptionTypeEnum.FULL);
        }
        #endregion
    }
}
