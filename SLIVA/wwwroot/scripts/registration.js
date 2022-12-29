function SendData(){
    var u_name  = document.getElementById('u_name').value;
    var u_pwd  = document.getElementById('u_pwd').value;
    var u_email  = document.getElementById('u_email').value;
    var u_login  = document.getElementById('u_login').value;
    console.log(u_login);
    console.log(u_email);
    console.log(u_pwd);
    console.log(u_name);
    fetch('http://localhost:8000/registration', {
    method: 'POST',
    headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({ 'Username': u_name, 'Login': u_login, 'Email': u_email, 'Password': u_pwd })
    })
    .then(response => response.json())
    .then(response => console.log(JSON.stringify(response)))
}