function SendData(){
    var u_pwd  = document.getElementById('u_pwd').value;
    var u_pwd2  = document.getElementById('u_pwd2').value;
    var u_login  = document.getElementById('u_login').value;
    console.log(u_login);
    console.log(u_pwd);
    if(u_pwd == u_pwd2){
        fetch('/registration', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ 'Login': u_login,'Password': u_pwd })
        })
        .then(response => response.json())
        .then(response => {
            let res = JSON.stringify(response).replaceAll('"','');
            if(res.toLowerCase() === 'done')
                document.location.href = '/forum';
                
        })
    }

    
}