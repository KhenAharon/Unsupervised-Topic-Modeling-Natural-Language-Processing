<?php
	session_start();
?>
<!DOCTYPE html>
<html lang="he" dir="rtl">
<head>
	<script>

	
	$(document).ready(function(){
		$("#details").hide();
		$("#hide").click(function(){
			$("#details").hide();
		});
		$("#show").click(function(){
			$("#details").show();
		});
	});
	
	</script>
	<style>
		.borderless td, .borderless th {
			border: none;
		}
		#th1 {
			width: 15%;
		}
		#th2 {
			width: 5%;
		}
		#th3 {
			width: 80%;
		}
	</style>

</head>
<body>

<?php
ini_set('max_execution_time', 5000);

error_reporting( E_ERROR | E_PARSE | E_CORE_ERROR | E_CORE_WARNING | E_COMPILE_ERROR | E_COMPILE_WARNING );
define ('SITE_ROOT', realpath(dirname(__FILE__)));

$target_file = basename($_SERVER['DOCUMENT_ROOT'].'/'.$_FILES["fileToUpload"]["name"]); //specifies the path of the file to be uploaded
$uploadOk = 1; //is not used yet (will be used later)
$imageFileType = strtolower(pathinfo($target_file,PATHINFO_EXTENSION)); //holds the file extension of the file (in lower case)
// Check if image file is a actual image or fake image
if (move_uploaded_file($_FILES["fileToUpload"]["tmp_name"], $target_file)) { //move_uploaded_file — Moves an uploaded file to a new location
	$flags_list = array();
	$tagger = 0;
	if (isset($_POST["keep-sequence"])) {
		array_push($flags_list, $_POST["keep-sequence"]);
	}
	if (isset($_POST["stoplist"])) {
		array_push($flags_list, $_POST["stoplist"]);
	}
	if (isset($_POST["tagger"])) {
		$tagger = 1;
	}
	//create json to send server
	$obj->path = $_SERVER['DOCUMENT_ROOT'].'/'.$_FILES["fileToUpload"]["name"];
	$obj->topics = $_POST["range_num"];
	$obj->flags = $flags_list;
	$obj->tagger = $tagger;
	$json= json_encode($obj);
	$matan_answered = 0;
	$flag = 1;
	$numberOfTopics = (int)$obj->topics;
	$dataPoints= array();
	$graph = array_fill(0,((int)$obj->topics), 0);
	//wait for answer from server
	while(!$matan_answered) {
		$fp = fsockopen("localhost", 8888, $errno, $errstr, 30);
		if (!$fp) {
			echo "$errstr ($errno)<br />\n";
			$matan_answered = 1;
		} else {
			if($flag==1)
			{
			   fwrite($fp, $json);
		       $flag = 0;
		    }			 
			$all = "";
			while (!feof($fp)) {
				$all .= fgets($fp, 2000);
			}
			file_put_contents('matan_json1.txt', $all);
			//while sending data
			if (($all != null) and ($all != "no")) {
				$matan_answered = 1;
				$topics = json_decode($all);
				$topics_array = array();
				//create tables with data
				echo "<table class='table table-sm borderless table-hover' dir='rtl'>";
				echo "<thead class='thead-light'><tr><th>נושאים:</th><th></th></tr></thead>";
				for ($x = 0; $x < count($topics); $x++) {
					$words = array();
					$cur = $topics[$x];
					echo '<tr><td>'.$cur->id.'</td>';
					$row_keys = $cur->keys;
					echo '<td>';
					for($y = 0; $y < count($row_keys) - 1; $y++){
						echo $row_keys[$y].', ';
						array_push($words, $row_keys[$y]);
					}
					//create keys
					array_push($words, $row_keys[$y]);
					array_push($topics_array, $words);
					echo $row_keys[$y].'</td>';
					echo '</tr>';
				}
				$_SESSION["topics_array"] = $topics_array;
				echo '</table>';
				
				$all="";
				$fp = fsockopen("localhost", 8888, $errno, $errstr, 30);
				while (!feof($fp)) {
					$all .= fgets($fp, 2000);
				}
				file_put_contents('matan_json2.txt', $all);
				
				
				
				//print data 
				
				print '<br><br>נושאים לכל מסמך:<br>';
				echo  '	<button id="show">הראה פרטים</button>
						<button id="hide">הסתר פרטים</button>';
				echo   "<table class='table table-sm borderless table-hover' dir='rtl' id='details' dir='rtl'>";
				echo  "<thead class='thead-light'><tr><th id='th1'>מסמך</th><th id='th2'>נושא/ים</th><th id='th3'>תצוגה מקדימה</th></tr></thead>";
				$docs = json_decode($all);
				$counter = 1;
				


				for ($x = 0; $x < count($docs); $x++) {
					$cur = $docs[$x];
					$name = $cur->name;
					echo '<tr><td><a target="_blank" href="index.php?p=4&no='.$counter.'" id="no'.$counter.'">'.$name.'</a></td>';
					$row_topics = $cur->topics;
					$percentage = $cur->percentage;
					echo '<td>';
					$specificTopics = array();
					for($y = 0; $y < count($row_topics) - 1; $y++){
						echo '<div class="tooltip2">'.$row_topics[$y].',&nbsp;';
						echo '<span class="tooltiptext2">'.((int)(((float)($percentage[$y]))*100)).'%</span>';
						echo '</div>';
						array_push($specificTopics, $row_topics[$y] - 1); //we begin from 0 in actual array.
						$graph[($row_topics[$y])-1]=($graph[($row_topics[$y])-1])+1;
					}
					array_push($specificTopics, $row_topics[$y] - 1); //we begin from 0 in actual array.
					echo '<div class="tooltip2">'.$row_topics[$y];
					echo '<span class="tooltiptext2">'.((int)(((float)($percentage[$y]))*100)).'%</span>';
					echo '</div>';
					echo '</td>';
					
					$preview = $cur->preview;
					$path = $cur->path;
					echo '<td>'.$preview.'</td></tr>';
					
					$session_name = 'no'.$counter;
					$_SESSION[$session_name] = $path;
					$_SESSION["specificTopicNumbers".$counter] = $specificTopics; //specificTopicNumbers1 = [1,3]
					$counter = $counter + 1;
				}
				echo '</table>';
			} else {
				sleep(1);
			}
			fclose($fp);
		}
	}
	
	  for($o=0;$o<(int)$obj->topics;$o++){
		     $mat=$graph[$o];
	         array_push($dataPoints,array("label"=> "Topic Id ".($o + 1), "y"=>$mat));
      }
	?>
<div class="row">
	<div class="col-sm-6">
		<div id="chartContainer" style="height: 370px;" dir="ltr"></div>
		<script src="js/canvasjs.min.js"></script>
		<script>
		var chart = new CanvasJS.Chart("chartContainer", {
			animationEnabled: true,
			exportEnabled: true,
			title:{
				text: "חלוקת קבצים לפי נושאים"
			},
			subtitles: [{
				text: "כמות קבצים בכל נושא"
			}],
			data: [{
				type: "pie",
				showInLegend: "true",
				legendText: "{label}",
				indexLabelFontSize: 16,
				indexLabel: "{label} - #percent%",
				yValueFormatString: "฿#,##0",
				dataPoints: <?php echo json_encode($dataPoints, JSON_NUMERIC_CHECK); ?>
			}]
		});
		chart.render();
		</script>
	</div>
	<div class="col-sm-6"></div>
</div>
<?php
} else {
	echo "סליחה, אבל היתה בעיה בעת העלאת הקובץ (האם בחרת קובץ?)";
	echo "<br>";
	echo '<a href="index.php?p=1"> חזור אחורה </a>';
}
?>

</body>
</html>
