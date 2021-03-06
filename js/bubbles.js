var canvas = document.getElementById('bgCanvas');
var score = 0;
var link = '31,154,207';
var hover = '221,132,44';
var bg = '9,46,109';
var bg2 = '0,159,255';
var text = '255,255,255';
var post = '230,230,230';
var textinner = '20,20,20';
var footer = '9,46,109';

document.addEventListener("DOMContentLoaded", () => {
	/* Set Performance from Cookie */
	SetPerformance();

	/* Set Theme from Cookie or Dropdown */
	SetTheme();

	/* Set Bubble Popping Score from Cookie */
	score = getCookie("score");
	document.getElementById('score').innerText = score;

	/* Set Bubble SFX Icon from Cookie*/
	SetSound();

	/* Spawn Bubbles */
	var c = document.getElementById('bgCanvas'),
		$ = c.getContext('2d'),
		w = c.width = window.innerWidth,
		h = c.height = window.innerHeight;

	var i, bubblesNumber = w * h > 750000 ? 50 : 10,
		objects = [],
		maxRadius = w * h > 500000 ? 20 : 15,
		maxYVelocity = 2;

	function randomInRange(min, max) {
		return Math.random() * (max - min) + min;
	}

	function Vector(x, y) {
		this.x = x || 0;
		this.y = y || 0;
	}

	Vector.prototype.add = function (v) {
		this.x += v.x;
		this.y += v.y;
		return this;
	};

	Vector.prototype.multiply = function (value) {
		this.x *= value;
		this.y *= value;
		return this;
	};

	Vector.prototype.getMagnitude = function () {
		return Math.sqrt(this.x * this.x + this.y * this.y);
	};

	function Fragment(position, velocity, radius, hue) {
		this.position = position;
		this.velocity = velocity;
		this.startSpeed = this.velocity.getMagnitude();
		this.radius = radius;
		this.hue = hue;
	}

	Fragment.prototype.update = function (world) {
		this.velocity.multiply(world.physicalProperties.friction);
		this.position.add(this.velocity);
		this.radius *= this.velocity.getMagnitude() / this.startSpeed;
		if (this.radius < 0.1) {
			world.objects.splice(world.objects.indexOf(this), 1);
		}
	}

	Fragment.prototype.render = function ($) {
		$.beginPath();
		$.fillStyle = 'rgba(' + text + ',' + this.opacity + ')';
		$.arc(this.position.x, this.position.y, this.radius, 0, Math.PI * 2);
		$.fill();
	};

	function Bubble(x, y, speed, radius, fragments, swing, hue) {
		this.x = x;
		this.y = y;
		this.startX = this.x;
		this.speed = speed;
		this.radius = radius;
		this.fragments = fragments;
		this.swing = swing;
		this.hue = hue;
		this.opacity = randomInRange(0.01, 0.2);
	}

	Bubble.prototype.update = function (world) {
		this.x = this.startX + Math.cos(this.y / 80) * this.swing;
		this.y += this.speed;
		if (this.y + this.radius < 0) {
			this.y = world.physicalProperties.height + this.radius;
		}
	}

	Bubble.prototype.render = function ($) {
		$.beginPath();
		$.fillStyle = 'rgba(' + text + ',' + this.opacity + ')';
		$.arc(this.x, this.y, this.radius, 0, 2 * Math.PI);
		$.fill();
	};

	Bubble.prototype.pop = function (world) {
		world.objects.splice(world.objects.indexOf(this), 1);
		for (var i = 0; i < this.fragments; i++) {
			world.objects.push(new Fragment(new Vector(this.x, this.y), new Vector(randomInRange(-2, 2), randomInRange(-2, 2)), randomInRange(2, this.radius / 4), this.hue));
		}
		objects.push(new Bubble(Math.random() * w, Math.random() * h, -randomInRange(0.5, maxYVelocity), randomInRange(5, maxRadius), randomInRange(1, 10), randomInRange(-40, 40), randomInRange(180, 200)));
		score++;
		setCookie("score", score, 999);
		document.getElementById('score').innerText = score;
		var sound = getCookie("sound");
		if (sound == "1") {
			const rndInt = Math.floor(Math.random() * 4) + 1;
			var audio = new Audio('https://shrinefox.github.io/wav/pop' + rndInt + '.wav');
			audio.play();
		}
		if (score == 420) {
			window.open(
				"https://www.youtube.com/watch?v=SEDpQy5EPR4&list=PLU6By7bu-RSsNY_qWdA0EfpT-_em7u_i9", "_blank");
		}
	};

	function World(physicalProperties, objects, ctx, background) {
		this.physicalProperties = physicalProperties;
		this.objects = objects;
		this.ctx = ctx;
		this.background = background;
		this.frameID = 0;
	}

	World.prototype.update = function () {
		for (var i = 0; i < this.objects.length; i++) {
			this.objects[i].update(this);
		}
	};

	World.prototype.render = function () {
		this.ctx.clearRect(0, 0, this.physicalProperties.width, this.physicalProperties.height);
		if (this.background) {
			this.ctx.fillStyle = this.background;
		}
		for (var i = 0; i < this.objects.length; i++) {
			this.objects[i].render(this.ctx);
		}
	};

	World.prototype.animate = function () {
		this.update();
		this.render();
		this.frameID = requestAnimationFrame(this.animate.bind(this));
	};

	for (i = 0; i < bubblesNumber; i++) {
		objects.push(new Bubble(Math.random() * w, Math.random() * h, -randomInRange(0.5, maxYVelocity), randomInRange(5, maxRadius), randomInRange(1, 10), randomInRange(-40, 40), randomInRange(180, 200)));
	}

	var world = new World({
		width: c.width,
		height: c.height,
		friction: 0.997
	}, objects, $, 'rgba(0, 0, 0, 0)');

	$.globalCompositeOperation = 'lighter';

	world.animate();

	window.addEventListener('resize', function () {
		w = world.physicalProperties.width = c.width = window.innerWidth;
		h = world.physicalProperties.height = c.height = window.innerHeight;
		$.globalCompositeOperation = 'lighter';
	});

	window.addEventListener('mousemove', function (e) {
		for (var i = 0; i < world.objects.length; i++) {
			if ((world.objects[i] instanceof Bubble) && (e.clientX > world.objects[i].x - world.objects[i].radius && e.clientX < world.objects[i].x + world.objects[i].radius && e.clientY < world.objects[i].y + world.objects[i].radius && e.clientY > world.objects[i].y - world.objects[i].radius)) {
				world.objects[i].pop(world);
			}
		}
	});

	window.addEventListener('touchmove', function (e) {
		for (var i = 0; i < world.objects.length; i++) {
			if ((world.objects[i] instanceof Bubble) && (e.touches[0].clientX > world.objects[i].x - world.objects[i].radius && e.touches[0].clientX < world.objects[i].x + world.objects[i].radius && e.touches[0].clientY < world.objects[i].y + world.objects[i].radius && e.touches[0].clientY > world.objects[i].y - world.objects[i].radius)) {
				world.objects[i].pop(world);
			}
		}
	});

});

function SoundToggle() {
	if (document.getElementById("sound").classList.contains('fa-volume-mute')) {
		document.getElementById("sound").classList.remove('fa-volume-mute');
		document.getElementById("sound").classList.add('fa-volume-up');
		setCookie("sound", 1, 999);
		console.log("Set Sound: On");
	}
	else {
		document.getElementById("sound").classList.remove('fa-volume-up');
		document.getElementById("sound").classList.add('fa-volume-mute');
		setCookie("sound", 0, 999);
		console.log("Set Sound: Off");
	}
}

function SetSound() {
	if (getCookie("sound") == "1") {
		document.getElementById("sound").classList.remove('fa-volume-mute');
		document.getElementById("sound").classList.add('fa-volume-up');
		console.log("Sound: On");
	}
	else {
		document.getElementById("sound").classList.remove('fa-volume-up');
		document.getElementById("sound").classList.add('fa-volume-mute');
		console.log("Sound: Off");
	}
}

function PerformanceToggle() {
	if (document.getElementById("performance").classList.contains('fa-toggle-off')) {
		document.getElementById("performance").classList.remove('fa-toggle-off');
		document.getElementById("performance").classList.add('fa-toggle-on');
		setCookie("performance", 1, 999);
		console.log("Set Performance: On");
	}
	else {
		document.getElementById("performance").classList.remove('fa-toggle-on');
		document.getElementById("performance").classList.add('fa-toggle-off');
		setCookie("performance", 0, 999);
		console.log("Set Performance: Off");
	}

	SetPerformance();
}

function SetPerformance() {
	var c = document.getElementById('bgCanvas');

	if (getCookie("performance") == "1") {
		/* Toggle Icon */
		document.getElementById("performance").classList.remove('fa-toggle-off');
		document.getElementById("performance").classList.add('fa-toggle-on');
		console.log("Performance: On");

		/* Toggle Background */
		c.setAttribute("style", "display: none;");
		document.getElementsByClassName("flex")[0].setAttribute("style", "background: linear-gradient(120deg, var(--gradient1) 0%, var(--gradient2) 100%) fixed;");

		/* Toggle Wave Animation */
		for (var i = 0; i < 4; i++) {
			document.getElementsByTagName("use")[i].classList.add('noanimation');
			document.getElementsByTagName("use")[i].setAttribute("x", 25 * i + i);
		}
		c.offsetHeight; /* Reflow CSS */
	}
	else {
		/* Toggle Icon */
		document.getElementById("performance").classList.remove('fa-toggle-on');
		document.getElementById("performance").classList.add('fa-toggle-off');
		console.log("Performance: Off");

		/* Toggle Background */
		c.setAttribute("style", "display: initial;");
		document.getElementsByClassName("flex")[0].setAttribute("style", "background: transparent;");

		/* Toggle Wave Animation */
		for (var i = 0; i < 4; i++) {
			document.getElementsByTagName("use")[i].classList.remove('noanimation');
			document.getElementsByTagName("use")[i].setAttribute("x", 48);
		}
		c.offsetHeight; /* Reflow CSS */
	}
}

function ThemeToggle() {
	var theme = document.getElementById("theme").value.toLowerCase();
	setCookie("theme", theme, 999);

	SetTheme();
}

function SetTheme() {
	var game = document.getElementById("game").value.toLowerCase();
	var theme = getCookie("theme");
	console.log("Theme: " + theme);
	selectElement("theme", theme);
	HideColorPicker();

	/* Override amicitia.github.io game themeing with selection */
	if (theme == "") { theme = game; }
	if (theme == "p5" || theme == "p5r" || theme == "p5d" || theme == "p5s" || theme == "smt3") {
		link = '190,47,47';
		hover = '255,0,0';
		bg = '10,10,10';
		bg2 = '190,47,47';
		text = '255,255,255';
		post = '20,20,20';
		textinner = '230,230,230';
		footer = '10,10,10';
	}
	else if (theme == "p4" || theme == "p4g" || theme == "p4d" || theme == "p4au") {
		link = '89,57,0';
		hover = '255,175,57';
		bg = '89,57,0';
		bg2 = '255,175,57';
		text = '255,255,255';
		post = '230,230,230';
		textinner = '20,20,20';
		footer = '89,57,0';
	}
	else if (theme == "p3fes" || theme == "p3p" || theme == "p3d") {
		link = '4,112,0';
		hover = '9,255,0';
		bg = '0,0,0';
		bg2 = '2,43,0';
		text = '255,255,255';
		post = '0,0,0';
		textinner = '200,200,200';
		footer = '0,0,0';
	}
	else if (theme == "cfb" || theme == "pq" || theme == "pq2") {
		link = '185,95,165';
		hover = '255,55,155';
		bg = '83,9,88';
		bg2 = '255,135,185';
		text = '255,255,255';
		post = '230,230,230';
		textinner = '20,20,20';
		footer = '83,9,88';
	}
	else if (theme == "custom") {
		ShowColorPicker();
		/* Load color values from cookie */
		if (getCookie("color_link") == "") {
			setCookie("color_link", "255,255,255", 999);
			setCookie("color_hover", "251,158,59", 999);
			setCookie("color_bg", "0,0,0", 999);
			setCookie("color_bg2", "255,255,255", 999);
			setCookie("color_text", '255,255,255', 999);
			setCookie("color_post", '230,230,230', 999);
			setCookie("color_textinner", '20,20,20', 999);
			setCookie("color_footer", "0,0,0", 999);
		}
		link = getCookie("color_link");
		hover = getCookie("color_hover");
		bg = getCookie("color_bg");
		bg2 = getCookie("color_bg2");
		text = getCookie("color_text");
		post = getCookie('color_post');
		textinner = getCookie("color_textinner");
		footer = getCookie("color_footer");
	}
	else {
		/* Default Theme */
		link = '31,154,207';
		hover = '221,132,44';
		bg = '9,46,109';
		bg2 = '0,159,255';
		text = '255,255,255';
		post = '230,230,230';
		textinner = '20,20,20';
		footer = '9,46,109';
	}

	/* Override CSS color values */
	document.documentElement.style.setProperty('--link', link);
	document.documentElement.style.setProperty('--bg', bg);
	document.documentElement.style.setProperty('--bg2', bg2);
	document.documentElement.style.setProperty('--hover', hover);
	document.documentElement.style.setProperty('--text', text);
	document.documentElement.style.setProperty('--textinner', textinner);
	document.documentElement.style.setProperty('--post', post);
	document.documentElement.style.setProperty('--footer', footer);
}

function updateLink(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_link", rgb, 999);
	SetTheme();
}

function updateHover(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_hover", rgb, 999);
	SetTheme();
}

function updateBg(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_bg", rgb, 999);
	SetTheme();
}

function updateBg2(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_bg2", rgb, 999);
	SetTheme();
}

function updatePost(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_post", rgb, 999);
	SetTheme();
}

function updateText(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_text", rgb, 999);
	SetTheme();
}

function updateTextinner(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_textinner", rgb, 999);
	SetTheme();
}

function updateFooter(picker) {
	var rgb = picker.toRGBString().replace("rgb(", "").replace(")", "");
	setCookie("color_footer", rgb, 999);
	SetTheme();
}

function updateColorPicker() {
		linkstring = 'rgba(' + link + ',1);'
		hoverstring = 'rgba(' + hover + ',1);'
		bgstring = 'rgba(' + bg + ',1);'
		bg2string = 'rgba(' + bg2 + ',1);'
		textstring = 'rgba(' + text + ',1);'
		poststring = 'rgba(' + post + ',1);'
		textinnerstring = 'rgba(' + textinner + ',1);'
		footerstring = 'rgba(' + footer + ',1);'
		
		document.querySelector('#customlink').jscolor.fromString(linkstring);
		document.querySelector('#customhover').jscolor.fromString(hoverstring);
		document.querySelector('#custombg').jscolor.fromString(bgstring);
		document.querySelector('#custombg2').jscolor.fromString(bg2string);
		document.querySelector('#customtext').jscolor.fromString(textstring);
		document.querySelector('#custompost').jscolor.fromString(poststring);
		document.querySelector('#customtextinner').jscolor.fromString(textinnerstring);
		document.querySelector('#customfooter').jscolor.fromString(footerstring);
}

function getCookie(cname) {
	var name = cname + "=";
	var decodedCookie = decodeURIComponent(document.cookie);
	var ca = decodedCookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') {
			c = c.substring(1);
		}
		if (c.indexOf(name) == 0) {
			return c.substring(name.length, c.length);
		}
	}
	return "";
}

function setCookie(cname, val, exdays) {
	var exdate = new Date();
	exdate.setDate(exdate.getDate() + exdays);
	var c_value = escape(val) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
	document.cookie = cname + "=" + c_value + ";path=/";
}

function selectElement(id, valueToSelect) {
	let element = document.getElementById(id);
	element.value = valueToSelect;
}

function ShowColorPicker() {
	var c = document.getElementById('colorpicker');
	c.setAttribute("style", "display: block;");
}

function HideColorPicker() {
	var c = document.getElementById('colorpicker');
	c.setAttribute("style", "display: none;");
}