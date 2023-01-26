function SendData(){
    var u_pwd  = document.getElementById('u_pwd').value;
    var u_login  = document.getElementById('u_login').value;
    console.log(u_login);
    console.log(u_pwd);
    var http_response;
    fetch('/authentication', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
    body: JSON.stringify({'Login': u_login,'Password':u_pwd })
    })
    .then(response => response.json())
    .then(response => {
        let res = JSON.stringify(response).replaceAll('"','');
        console.log(res);
        console.log(res.toLocaleLowerCase() === "succes");
        if(res.toLowerCase() === 'succes')
            document.location.href = '/forum';
            
    })
}