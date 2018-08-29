<?php
// Start the session
session_start();
?>
<style>
	.color0{
		background-color: #FFFF00;
	}
	.color1{
		background-color: #00FF00;
	}
	.color2{
		background-color: #00FFFF;
	}
	.color3{
		background-color: #FFA500;
	}
	.color4{
		background-color: #FF00FF;
	}
</style>
<?php
$no = $_GET["no"];
$session_name = "no".$no;
$path = $_SESSION[$session_name];

$topics_array = $_SESSION["topics_array"];
$specificTopicNumbers = $_SESSION["specificTopicNumbers".$no];
$all_words = array();
$colors_wheel = 5;
$my_color = 0;

for ($x = 0; $x < count($specificTopicNumbers); $x++) {
	$a_topic_number = $specificTopicNumbers[$x];
	$specific_topic_words = $topics_array[$a_topic_number];
	for ($y = 0; $y < count($specific_topic_words); $y++) {
		$word = $specific_topic_words[$y];
		array_push($all_words, array($word, $my_color % $colors_wheel)); //word, color
	}
	$my_color = $my_color + 1;
}
echo "<br><h1>הצגת מסמך</h1><br>";
echo "<h3>".$path."</h3>";
$myfile = fopen($path, "r") or die("Unable to open file!");
$document = fread($myfile,filesize($path));
$pieces = explode(" ", $document);

for ($i = 0; $i < count($pieces); $i++) {
	// for each piece
	$bold = 0;
	for ($j = 0; ($j < count($all_words)) && (!$bold); $j++) {
		if ($pieces[$i] == $all_words[$j][0]) {
			$bold = 1;
			$color = $all_words[$j][1];
		}
	}
	if($bold) {
		echo '<mark class="color'.$color.'">'.$pieces[$i].'</mark> ';
	}
	else {
		echo $pieces[$i].' ';
	}
}
echo "<span id='document'>".fread($myfile,filesize($path))."</span>";
fclose($myfile);

?>

<?php 
	/*for ($i = 0; $i < count($all_words); $i++) {
		echo $all_words[$i].' ';
	}
	print_r($specificTopicNumbers);
	*/
?>
