jQuery(function($){
	
	/*==Window resize==*/
	var id;
	$(window).resize(function() {
		clearTimeout(id);
		id = setTimeout(doneResizing, 500);
	});
	function doneResizing(){
		var windowWidth=$(window).width(),nav=$('#nav');
		
		nav.unstick();
		if(windowWidth>=992){
			nav.sticky({topSpacing:0,className:'fix-menu'});
			$('.dropdown',nav).hover(function(){
				$(this).addClass('open');
			},function(){
				$(this).removeClass('open');
			});
		}
		
		if(windowWidth < 768){
			$('.table-toggle').click(function(e){
				e.preventDefault();
				$(this).parents('thead').next('tbody').children('tr ~ tr').slideToggle('100');
			});
		}
	}
	doneResizing();
	/*//==Window resize==*/
	
	
	
	
	/*==currency==*/
	if($('.currency').length>0){
		webshims.setOptions('forms-ext', {
			replaceUI: 'auto',
			types: 'number'
		});
		webshims.polyfill('forms forms-ext');
	}
	/*//==currency==*/
	
	
	$('#nav .open-menu').click(function(){$('#nav').toggleClass('open-menu');});
	
	$('.toggle-sidebar').click(function(){$('.left-sidebar').toggleClass('open-menu');});
	
	
	
	/*==Product view==*/
	$('.product-view a').click(function(e){
		e.preventDefault();
		var pb=$('.product-box'),gp=$('.grid-product'),block=$('.grid-col-block');
		
		if($(this).hasClass('list-product')){
			pb.addClass('list-view');
			$('.list-product').addClass('active');
			gp.removeClass('active');
			block.toggleClass('list-col-view');
		}else{
			pb.removeClass('list-view');
			$('.list-product').removeClass('active');
			gp.addClass('active');
			block.toggleClass('list-col-view');
		}
	});
	/*//==Product view==*/
	
	/*==OwlSlider==*/
	if($("#owl-slider").length>0){
		$("#owl-slider").owlCarousel({
			autoPlay: 3000,
			items : 5,
			itemsDesktop : [1199,4],
			itemsDesktopSmall : [991,3],
			navigation:true,
		    navigationText: [
		      "<i class='fa fa-angle-left'></i>",
		      "<i class='fa fa-angle-right'></i>"
		      ]
		});
	}
	/*==OwlSlider==*/
	
	/*==Table collapse==*/
	$('.expand-btn').click(function(e){
		e.preventDefault();
		$('.table-collapse').fadeIn('200');
	});
	$('.colapse-btn').click(function(e){
		e.preventDefault();
		$('.table-collapse').fadeOut('100');
	});
	/*==//Table collapse==*/
	
	
	/*==Load file==*/
	$('.file-load-btn').click(function(){
		$('+span+input[type=file]',this).click();
		loadChange(this);
		return false;
	});
	function loadChange(m){
		$('+.name-load-file+input[type=file]',m).change(function() {
	    	$('+.name-load-file',m).text($(this).val());
		});
	}
	/*==//Load file==*/
	
	
	/*==Faqs==*/
    $('#text-search').bind('keyup', function(ev) {
        var searchTerm = $(this).val();
        
        if(searchTerm.length>1){
        	$('.panel-heading').each(function(){
        		if(!!($(this).text().toUpperCase().search(searchTerm.toUpperCase())+1)){
					$(this).parents('.panel').show();
				}else{
					$(this).parents('.panel').hide();
				}
        	});
        }else{
			$('.panel').show();
		}
    });
	/*==//Faqs==*/
	

});

/*==Slider==*/
if($("#slider").length>0){
	(function(){
	  var flexslider;
	  function getGridSize() {
	    return (window.innerWidth < 767) ? 2 :
	           (window.innerWidth < 991) ? 3 : 4;
	  }
	 
	  $(window).load(function() {  	
	    $('#carousel').flexslider({
			animation: "slide",
			itemWidth: 385,
			minItems: getGridSize(),
			maxItems: getGridSize(),
			controlNav: false,
			asNavFor: '#slider',
			start: function(slider){
				flexslider = slider;
			}
	    });
	    
	    $('#slider').flexslider({
			animation: "slide",
			controlNav: false,
			animationLoop: false,
			slideshow: false,
			sync: "#carousel"
		});
	  });

	  $(window).resize(function() {
	    getGridSize();
	  });
	}());
}
/*==Slider==*/