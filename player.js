document.addEventListener("DOMContentLoaded", function(){

    //玩家資料區塊
    const playerDataContainer = document.querySelector(".player-data");
    const editProfileContainer = document.querySelector(".edit-profile");
    const editProfileButton = document.getElementById("editProfileButton");
    const logoutButton = document.getElementById("logoutButton");

    const playerNameInput = document.querySelector('input[name="playerName"]');
    const playerNameDisplay = document.querySelector(".player-data h2");

    const emailInput = document.querySelector('input[name="email"]');
    const emailDisplay = document.querySelector(".player-email");

    const birthdayInput = document.querySelector('input[name="birthday"]');
    const birthdayDisplay = document.querySelector(".player-birthday");

    const genderInputs = document.querySelectorAll('input[name="gender"]');
    const genderDisplay = document.querySelector(".gender");

    const playerIdDisplay = document.querySelector(".player-id");
    const playerLevelDisplay = document.querySelector(".player-level");
    const genderLabels ={
        male: "男性",
        female: "女性",
        other: "其他"
    };

    let playerData = null;

    // 檢查登入狀態
    let playerEmail = sessionStorage.getItem("playerEmail");
    if(sessionStorage.getItem("isLoggedIn")!=="true"){
        window.location.href="index.html";
        return;
    };

    // 載入玩家資料
    fetch("http://localhost:5012/api/player?email=" + encodeURIComponent(playerEmail))
        .then(async function(response){
            if (!response.ok){
                const errorMessage = await response.text();
                throw new Error(errorMessage);
            }
            return response.json();
        })

        .then(function(playerDataResponse){

            playerData = playerDataResponse;

            playerNameDisplay.textContent = playerDataResponse.playerName;
            playerIdDisplay.textContent = playerDataResponse.playerId;
            emailDisplay.textContent = playerDataResponse.email;
            birthdayDisplay.textContent = playerDataResponse.birthday;     
            playerLevelDisplay.textContent = playerDataResponse.level;

            genderDisplay.textContent = genderLabels[playerDataResponse.gender] ?? "未設定";

            genderInputs.forEach(function(genderInput){
                if (genderInput.value === playerDataResponse.gender){
                    genderInput.checked = true;
                }
            });
        })

        .catch(function(error){
            console.error("載入玩家資料失敗", error);
            alert("無法載入玩家資料:" + error.message)
        });
    
    //修改玩家資料
    if (editProfileButton){
        editProfileButton.addEventListener("click", function (event) {

        if (!playerData){
            return;
        }

        playerNameInput.value = playerData.playerName;
        emailInput.value = playerData.email;
        birthdayInput.value = playerData.birthday ??"";

        genderInputs.forEach(function(selectedGender){
            selectedGender.checked = 
                selectedGender.value === playerData.gender;
        });


        playerDataContainer.style.display = "none";
        editProfileContainer.style.display = "block";
        });
    };

    const editProfileForm = document.getElementById("editProfileForm");

        if (editProfileForm) {
            editProfileForm.addEventListener("submit",function(event){
                event.preventDefault();

                if(!playerData){
                    return;
                }

                let selectedGender = "";            

                genderInputs.forEach(function(genderInput){
                    if(genderInput.checked){
                        selectedGender = genderInput.value;
                    };
                });

                const updatedPlayerData={
                    playerName: playerNameInput.value,
                    email: emailInput.value,
                    birthday: birthdayInput.value,
                    gender: selectedGender
                }; 


                const requestBody = JSON.stringify(updatedPlayerData);

                fetch("http://localhost:5012/api/player?email="+ encodeURIComponent(playerEmail),{
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: requestBody
                })
                .then(async function(response){

                    if(!response.ok){
                        const errorMessage = await response.text();

                        console.error("儲存失敗的詳細原因：", errorMessage);

                        throw new Error(errorMessage);
                    }

                    return response.json();
                })
                .then(function(updatedPlayerData){
                    playerData = updatedPlayerData;
                    sessionStorage.setItem("playerEmail", updatedPlayerData.email);
                    playerEmail = updatedPlayerData.email;

                    playerNameDisplay.textContent = updatedPlayerData.playerName;
                    emailDisplay.textContent = updatedPlayerData.email;
                    birthdayDisplay.textContent = updatedPlayerData.birthday;
                    genderDisplay.textContent = genderLabels[updatedPlayerData.gender] ?? "未設定";

                    editProfileContainer.style.display = "none";
                    playerDataContainer.style.display = "block";

                })
                .catch(function(error){

                    console.error("儲存玩家資料失敗", error);
                    alert(error.message);

                });
            });
        }
    
    //修改密碼
    const changePasswordForm = document.getElementById("changePasswordForm");

        if (changePasswordForm) {
        changePasswordForm.addEventListener("submit", async function(event){
            event.preventDefault();

            const currentPassword = changePasswordForm.elements['currentPassword'].value;
            const newPassword = changePasswordForm.elements['newPassword'].value;
            const confirmPassword = changePasswordForm.elements['confirmPassword'].value;

            if (newPassword !== confirmPassword){
                alert("兩次輸入密碼不一致")
                return;
            }
            
            const email = sessionStorage.getItem("playerEmail");

            try {
                const response = await fetch(`http://localhost:5012/api/change-password?email=${encodeURIComponent(email)}`,
                {
                    method:"POST",
                    headers:{
                        "Content-Type":"application/json"
                    },

                    body:JSON.stringify({
                        currentPassword: currentPassword,
                        newPassword: newPassword
                    })
                }
            );

            const message = await response.text();

            if (!response.ok){
                alert(message);
                return;
            }

            alert(message);
            changePasswordForm.reset();
            }
            catch(error){
                console.error("修改密碼失敗:", error);
                alert("無法連接伺服器，請確認後端 API 是否正常啟動")
            }
        });
    };

    //登出
    if(logoutButton){
        logoutButton.addEventListener("click", function(){
            sessionStorage.removeItem("isLoggedIn");
            sessionStorage.removeItem("playerEmail");

            alert("已登出!");
            window.location.href="index.html";
        });
    }
});