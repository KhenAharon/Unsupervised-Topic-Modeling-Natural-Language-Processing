<?php
	if(!isset($p))
	{
		$p = 1;
	}
	if(!empty($_GET))
	{
		if($_GET["p"] != null){
			$p = $_GET["p"];
		}
	}
?>

<!DOCTYPE html>
<html lang="he" dir="rtl">
<head>
	<meta charset="utf-8">
	<title>עיבוד שפה טבעית - מידול נושאים</title>
	
	<!--CDN versions of jQuery and Popper.js for BOOTSTRAP-->
	<script src="js/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script>
	<script src="js/popper.min.js" integrity="sha384-cs/chFZiN24E4KMATLdqdvsezGxaGsi4hLGOzlXwp5UZB1LY//20VyM2taTB4QvJ" crossorigin="anonymous"></script>
	<script src="js/bootstrap.min.js" integrity="sha384-uefMccjFJAIv6A+rW+L4AHf99KvxDjWSu1z9VI8SKNVmz4sk7buKt/6v9KI65qnm" crossorigin="anonymous"></script>
	<!-- Bootstrap -->
	<link rel="stylesheet" href="css/bootstrap.min.css" integrity="sha384-9gVQ4dYFwwWSjIDZnLEWnxCjeSWFphJiwGPXr1jddIhOegiu1FwO5qRGvFXOdJZ4" crossorigin="anonymous">
	<link rel="stylesheet" href="css/style.css">

</head>
<body width="100%">

<nav class="navbar navbar-expand-lg" >
  <a class="navbar-brand" href="#">עיבוד שפה טבעית - מידול נושאים</a>
  <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
	<span class="navbar-toggler-icon"></span>
  </button>

  <div class="pull-right">
	<ul class="navbar-nav mr-auto">
	  <li class="nav-item <?php if($p == 1 || $p == 3 || $p == 4){echo 'active';}?>">
		<a class="nav-link" href="index.php?p=1">העלה קורפוס</a>
	  </li>
	  <li class="nav-item <?php if($p == 2){echo 'active';}?>">
		<a class="nav-link" href="index.php?p=2">אודותנו</a>
	  </li>
	</ul>
  </div>
</nav>

<br>
<?php
	switch ($p) {
	case 1:
		include 'upload.html';
		break;
    case 2:
		include 'about.html';
        break;
    case 3:
		include 'upload_action.php';
        break;
    case 4:
		if(isset($_GET["no"])){
			include 'load_doc.php';
		} else {
			echo 'מנסה לטעון מסמך אך לא סופק מספר.';
		}
		break;
    default:
		break;
	}
?>

<footer class="footer-basic-centered">
	<a href="#"> <span class="footer-company-motto">Unsupervised Topic Modeling</span> </a>
	<br>
	<span class="footer-company-name">נבנה על ידי חן אהרן ומתן חליפה · אוניברסיטת בר אילן &copy;</span>
</footer>
	

</body>
</html>