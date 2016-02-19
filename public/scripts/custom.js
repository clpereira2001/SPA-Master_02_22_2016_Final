function FloatToCurrency(amount) {
  var txt = "$" + amount;
  var l = txt.indexOf(".", 0);
  if (l == -1)
    txt = txt + ".00";
  else {
    l = txt.substring(txt.indexOf(".", 0) + 1, txt.length).length;
    switch (l) {
      case 1:
        txt = txt + "0";
        break;
      case 2:
        break;
      default:
        txt = txt.substring(0, txt.indexOf(".", 0) + 3);
        break;
    }
  }
  return txt;
}