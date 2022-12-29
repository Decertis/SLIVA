function SendData(){
    var u_pwd  = document.getElementById('u_pwd').value;
    var u_login  = document.getElementById('u_login').value;
    console.log(u_login);
    console.log(u_pwd);
    fetch('http://localhost:8000/authentication', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    body: JSON.stringify({'Login': u_login,'Password':u_pwd })
    })
    .then(response => response.json())
    .then(response => console.log(JSON.stringify(response)))
    
}