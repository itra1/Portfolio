<?php

$arr = array(
  'status' => 'ok', 
  'data' => array(
    'username' => 'itra', 
    'versions' => array()
  )
);
echo json_encode($arr, JSON_UNESCAPED_UNICODE | JSON_UNESCAPED_SLASHES | JSON_NUMERIC_CHECK);

?>