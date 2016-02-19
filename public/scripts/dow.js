function swap_image(img, imgID) {
  document.main_image.src = img;
  if (document.getElementById("imagepathList") != null) {
    var imagePathList = document.getElementById("imagepathList").value.split("|");
    for (var i = 0; i < imagePathList.length; i++) {
      if (imagePathList[i] == imgID) {
        document.getElementById(imgID).style.border = "2px solid #FFFFFF";
      }
      else {
        document.getElementById(imagePathList[i]).style.border = "";
      }
    }
  }
}
function validateNumeric(e) {
  var k;
  if (window.event) { // IE            
    k = e.keyCode;
  } else if (e.which) { // Netscape/Firefox/Opera            
    k = e.which;
  } if ((k == 0xA) || (k == 0xD)) {
    if (document.getElementById("quantity") != null && document.getElementById("quantity").value != "") {
      if (!document.getElementById("Itakeit").disabled) {
        document.getElementById('Itakeit').click();
      }
      else {
        return false;
      }
    }
    else {
      return false;
    }
  }
  if ((k >= 48 && k <= 57) || (k == 8)) {
    return k;
  }
  else return false;
}
function displayImage(imageToShow, auctionID) {
  if (document.getElementById("imagepathList") != null) {
    var imagePathList = document.getElementById("imagepathList").value.split("|");
    var currentImage = document.getElementById("main_image").src.substring(document.getElementById("main_image").src.lastIndexOf("/", document.getElementById("main_image").src.length - 1) + 1, document.getElementById("main_image").src.length);
    for (var i = 0; i < imagePathList.length; i++) {
      if (currentImage == imagePathList[i]) {
        if (imageToShow == "previous") {
          if (i < imagePathList.length - 4) {
            document.getElementById("imagesDiv").scrollLeft = document.getElementById("imagesDiv").scrollLeft - 160;
          }
          if (i > 0) {
            document.getElementById("main_image").src = "/public/Auctionimages/" + Math.floor(auctionID / 1000000) + "/" + Math.floor(auctionID / 1000) + "/" + Math.floor(auctionID) + "/" + imagePathList[i - 1];
            document.getElementById(imagePathList[i - 1]).style.border = "2px solid #FFFFFF";
            document.getElementById(imagePathList[i]).style.border = "";
          }
          return;
        }
        if (imageToShow == "next") {
          if (i > 3) {
            document.getElementById("imagesDiv").scrollLeft = document.getElementById("imagesDiv").scrollLeft + 160;
          }
          if (i < imagePathList.length - 1) {
            document.getElementById("main_image").src = "/public/Auctionimages/" + Math.floor(auctionID / 1000000) + "/" + Math.floor(auctionID / 1000) + "/" + Math.floor(auctionID) + "/" + imagePathList[i + 1];
            document.getElementById(imagePathList[i]).style.border = "";
            document.getElementById(imagePathList[i + 1]).style.border = "2px solid #FFFFFF";
          }
          return;
        }
      }
    }
  }
}
function updateTotal(q) {
  if (q == '') {
    document.getElementById('total').innerText = 'Enter quantity';
    document.getElementById('total').textContent = 'Enter quantity';
    document.getElementById('Itakeit').disabled = true;
    document.getElementById('error').style.display = 'none';
    return;
  }
  var amount = parseInt(totalquantity, 10);
  var message = 'Sorry, we have only ' + amount + ' item' + (amount > 1 ? 's' : '') + ' available.' + (amount > 1 ? ' Please enter a lower quantity.' : '');
  if (amount) {
    if (amount >= q && q > 0 && q <= 10000) {
      document.getElementById('total').textContent = document.getElementById('total').innerText = "$" + parseFloat(q * totalprice).toFixed(2);
      document.getElementById('error').style.display = 'none';
      document.getElementById('Itakeit').disabled = false;
    }
    else {
      document.getElementById('total').innerText = '---';
      document.getElementById('total').textContent = '---';
      document.getElementById('Itakeit').disabled = true;
      document.getElementById('error').style.display = 'block';
      document.getElementById('error').textContent = message;
      document.getElementById('error').innerText = message;
    }
  }
  else {
    document.getElementById('total').innerText = '---';
    document.getElementById('total').textContent = '---';
    document.getElementById('Itakeit').disabled = true;
    document.getElementById('error').style.display = 'block';
    document.getElementById('error').textContent = message;
    document.getElementById('error').innerText = message;
  }  
}