document.addEventListener("DOMContentLoaded", function(){
    const loginForm = document.getElementById("loginForm");
    const registerForm = document.getElementById("registerForm");
    
    if (loginForm){
    loginForm.addEventListener("submit", async function(event){
        event.preventDefault();

        const email = loginForm.querySelector("input[name='email']").value;
        const password = loginForm.querySelector("input[name='password']").value;

        if (!email||!password){
            alert("請輸入電子郵件和密碼");
            return;
        }

        try{
            const response = await fetch("http://localhost:5012/api/login",{
                method: "POST",
                headers:{
                    "Content-Type":"application/json"
                },
                body: JSON.stringify({
                    email: email,
                    password: password
                })
            });

            const responseData = await response.text();
                if (!response.ok){
                    alert(responseData);
                    return;
                }
                sessionStorage.setItem("isLoggedIn","true");
                sessionStorage.setItem("playerEmail", email);

                alert("登入成功!");
                window.location.href = "player.html";

        }   catch(error){
                console.error("請求登入失敗:",error);
                alert("無法連接登入伺服器，請確認 API 是否正在執行。");
        }
    });

    }

    if (registerForm){
    registerForm.addEventListener("submit",async function(event){
        event.preventDefault();

        const playerName = registerForm.querySelector('input[name="playerName"]').value;
        const email = registerForm.querySelector('input[name="email"]').value;
        const password = registerForm.querySelector('input[name="password"]').value;
        const confirmPassword = registerForm.querySelector('input[name="confirmPassword"]').value;
        const birthday = registerForm.querySelector('input[name="birthday"]').value;
        const selectedGender = registerForm.querySelector('input[name="gender"]:checked');
        const gender = selectedGender ? selectedGender.value:"";

        const newPlayerData = {
            playerName: playerName,
            email: email,
            password: password,
            birthday: birthday,
            gender: gender
        }

        if (!playerName || !email || !password || !birthday){
            alert("必須填寫此欄位");
            return;
        }

        if (password.length < 8){
            alert("密碼至少需要8個字元");
            return;
        }

        if (password !== confirmPassword){
            alert("兩次輸入的密碼不一致");
            return;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (!emailRegex.test(email)){
            alert("請輸入正確的電子郵件格式");
            return;
        }

        try{
            const response = await fetch("http://localhost:5012/api/register",{
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(newPlayerData)
            });

            const responseText = await response.text();

            if (!response.ok){
                alert(responseText);
                return;
            }

            alert("註冊成功!");

        }catch (error){
            console.error("註冊請求失敗:",error);
            alert("無法連接註冊伺服器，請確認 API 是否正在執行。");
            }
        });
    };
});