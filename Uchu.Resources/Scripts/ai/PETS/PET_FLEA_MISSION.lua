--This script is 545
--SprayFleas (Mission 109) Pets have fleas. Player must spray 1 dog, 1 cat, 1 goat w/ spraygun.
--DOG w/ fleas 3647 (terrier) (regular terrier 3264)
--CAT w/ fleas 3648 (regular cat 3265)
--GOAT w/ fleas 3649 (regular goat 3266)
--FLEA EFFECT = BehaviorEffect ID 280


local ResetFleas_Time = 5.0


--WHEN STARTUP MESSAGE IS RECEIVED, CALL THE "SetFleas" FUNCTION, AND SET IT TO TRUE
function onStartup(self) 

    SetFleas(self, true) --Call the function that sets the state of the fleas, and set the function's boolean (bSetFleaBoolean) to true.
    
end

--A FUNCTION TO SET THE STATE OF FLEAS, TRUE OR FALSE. 
function SetFleas(self, bSetFleas) 
    if (bSetFleas == true) then
    
        self:SetVar("HasFleas" , true)
                
        --Put the fleas back on the animal
        --self:AddSkill { skillID = 109 } 
        --self:ToggleActiveSkill { iSkillID = 109, bOn = true }        --BEHAVIOR_DESTINK
    
    else
    
        self:SetVar("HasFleas" , false)  
                
        --self:RemoveSkill { skillID = 109 } 
        --self:ToggleActiveSkill{ iSkillID = 32, bOn = true }        --BEHAVIOR_DESTINK

    end
    
end


--When I'm squirted with the watergun
function onSquirtWithWatergun( self, msg )
    print ("Pet is wet.")
    
    --If I don't have fleas (skillBehavior 109 = Stink Skill)
    --if ( self:HasSkill { iSkillID = 109 }.bHas == false ) then
    if ( self:GetVar("HasFleas") == false) then
        print ("I don't have fleas.")
        return
    end

       
    --Otherwise, I must have fleas, and squirting feels good
    print ("Fleas are gone!")
           
    
    SetFleas(self, false) --Call the function that sets the state of the fleas, and set the function's boolean (bSetFleaBoolean) to false.
    
    local player = msg.shooterID --create a local variable called "player" and assign it to the shooterID from the onSquirtWithWatergun message
    
    player:UpdateMissionTask {target = self, value = 109, taskType = "complete"} -- update the mission task for mission 109 A value of 1 is sent to script owner, which increments the mission task by 1.
    
    --Start timer for fleas to return
    GAMEOBJ:GetTimer():AddTimerWithCancel( ResetFleas_Time , "Timer_ResetFleas", self ) -- Timer named Timer_ResetFleas is attached the script's "self" (the pet)
    
    
end


function onTimerDone (self , msg)
    if msg.name == "Timer_ResetFleas" then
    
        SetFleas(self, true) --Call the function that sets the state of the fleas, and set the function's boolean (bSetFleaBoolean) to true.
    
    end 

end