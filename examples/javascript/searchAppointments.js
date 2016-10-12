// jQuery 3.1.0
// https://cdnjs.cloudflare.com/ajax/libs/jquery/3.1.1/jquery.min.js
// ES5

var _basePath = 'https://[yourServerName]/SRSAPI/Generic/';

function login(userName, password) {
  return $.ajax({
    url: _basePath + 'Token',
    data: {
    	userName: userName,
      password: password,
      dataSourceId:0
    },
    type: "POST",
    datatype: 'json'
  }).catch(function(ex){
    console.log(ex);
    alert('Login Failed');
  });
}

function searchAppointments(token, personId){
  return $.ajax({
    //Find all appointments by PersonId
    url: _basePath + 'Appointment?PersonId=' + personId,
    type: "GET",
    datatype: 'json'
  }).catch(function(ex){
    console.log(ex);
    alert('Request Failed');
  });
}

login('yourUserName', 'yourPassword')
.then(function(token){
    return searchAppointments(token, 9783);
})
.then(function(results){
    //The API data
	console.log(results);
});




