# Export & Import Logs Test Plan

<table>
  <thead>
    <tr>
      <th>Feature</th>
      <th>Scenario / Description</th>
      <th>Input Parameters</th>
      <th>Test Data</th>
      <th>Expected Result</th>
    </tr>
  </thead>
  <tbody>
    <!-- Export Logs (7 rows) -->
    <tr>
      <td rowspan="7"><strong>Export Logs</strong></td>
      <td>Invalid Integer format</td>
      <td rowspan="7">Index</td>
      <td><code>ab, $, +</code></td>
      <td>ERROR: Invalid integer format</td>
    </tr>
    <tr>
      <td>Index out of range (negative)</td>
      <td><code>-1 or any value below 0</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Index out of range (exceeds count)</td>
      <td><code>above activities.Count</code></td>
      <td>ERROR: Index out of range</td>
    </tr>
    <tr>
      <td>Valid index, no logs recorded for the activity</td>
      <td><code>1</code></td>
      <td>ERROR: No logs recorded for the activity to export</td>
    </tr>
    <tr>
      <td>Valid index, only running logs exist (no stopped logs)</td>
      <td><code>1</code></td>
      <td>ERROR: No completed logs to export. Stop the running timer first.</td>
    </tr>
    <tr>
      <td>Valid index, mix of stopped and running logs</td>
      <td><code>1</code></td>
      <td>SUCCESS: Completed logs exported; running logs skipped</td>
    </tr>
    <tr>
      <td>Valid index, completed activity logs recorded (including commas in name &amp; &gt;24h duration)</td>
      <td><code>1</code></td>
      <td>SUCCESS: Logs were exported successfully</td>
    </tr>
    <!-- Import Logs (14 rows) -->
    <tr>
      <td rowspan="14"><strong>Import Logs</strong></td>
      <td>Index out of range or invalid file extension</td>
      <td rowspan="14">Index, File Path (string)</td>
      <td><code>-1, filename without .csv</code></td>
      <td>ERROR: Index out of range / ERROR: Invalid file path</td>
    </tr>
    <tr>
      <td>Valid index, valid file path (quoted via Windows Copy as Path)</td>
      <td><code>1, "C:\path\ChessActivity.csv"</code></td>
      <td>SUCCESS: Quotes trimmed and logs imported successfully</td>
    </tr>
    <tr>
      <td>Valid index, valid file path (no activities found for user)</td>
      <td><code>1, ChessActivity.csv</code></td>
      <td>ERROR: No tasks found to import logs</td>
    </tr>
    <tr>
      <td>Valid index, valid file path (file does not exist)</td>
      <td><code>1, MissingFile.csv</code></td>
      <td>ERROR: The specified file was not found</td>
    </tr>
    <tr>
      <td>Empty or header-only CSV file</td>
      <td><code>1, EmptyOrHeaderOnly.csv</code></td>
      <td>ERROR: The CSV file is empty or missing log entries</td>
    </tr>
    <tr>
      <td>Invalid CSV header columns</td>
      <td><code>1, WrongHeader.csv</code> (e.g., <code>Name, State, Start, End, Time</code>)</td>
      <td>ERROR: Invalid CSV format: Header does not match expected format</td>
    </tr>
    <tr>
      <td>Wrong column count on a data row</td>
      <td><code>1, WrongColumnCount.csv</code> (e.g., 4 or 6 unquoted columns)</td>
      <td>ERROR: Invalid CSV format at line N: Expected 5 columns</td>
    </tr>
    <tr>
      <td>Empty or whitespace-only Log Name</td>
      <td><code>1, EmptyLogName.csv</code> (e.g., <code>"   ", Stopped, ...</code>)</td>
      <td>ERROR: Invalid data format at line N: Log Name cannot be null or empty</td>
    </tr>
    <tr>
      <td>Invalid or non-Stopped Status</td>
      <td><code>1, BadStatus.csv</code> (e.g., <code>Running</code> or <code>Paused</code>)</td>
      <td>ERROR: Invalid data format at line N: Status must be 'Stopped'</td>
    </tr>
    <tr>
      <td>Bad Start Time or End Time format</td>
      <td><code>1, BadDate.csv</code> (e.g., <code>25/11/2026</code> instead of <code>yyyy-MM-dd HH:mm:ss</code>)</td>
      <td>ERROR: Invalid data format at line N: Invalid Start/End Time</td>
    </tr>
    <tr>
      <td>End Time earlier than or equal to Start Time</td>
      <td><code>1, EndBeforeStart.csv</code> (<code>EndTime &lt;= StartTime</code>)</td>
      <td>ERROR: Invalid data format at line N: End Time must be after Start Time</td>
    </tr>
    <tr>
      <td>Bad Duration format or mismatch with <code>EndTime - StartTime</code></td>
      <td><code>1, BadDuration.csv</code> (e.g., <code>25:00:00</code> or duration disagreeing with timestamps)</td>
      <td>ERROR: Invalid data format at line N: Invalid Duration / Duration does not match timestamps</td>
    </tr>
    <tr>
      <td>Valid CSV data, but timestamps overlap with existing logs</td>
      <td><code>1, OverlappingLogs.csv</code></td>
      <td>ERROR: No logs were imported because time overlaps with existing activity logs</td>
    </tr>
    <tr>
      <td>Valid CSV data with no overlapping entries (including quoted names with commas &amp; &gt;24h duration)</td>
      <td><code>1, ValidLogs.csv</code></td>
      <td>SUCCESS: Logs were imported successfully</td>
    </tr>
  </tbody>
</table>
