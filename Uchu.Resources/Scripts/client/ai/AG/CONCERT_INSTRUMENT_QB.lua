--require('o_mis')

---------------------------------------------------------------------------------------------------------
-- Client-side script for Concert instrument Quick Builds. When the instruments are built, they send a message
-- to the zone script, which will then lock the mini-fig into an imagination-draining "playing the instrument" 
-- animation until the player exits or runs out of imagination. The instrument's music track will be turned on 
-- for the duration.

---------------------------------------------------------------------------------------------------------
local CONSTANTS={}
CONSTANTS["GUITAR_LOT"]=4039
CONSTANTS["BASS_LOT"]=4040
CONSTANTS["KEYBOARD_LOT"]=4041
CONSTANTS["DRUM_LOT"]=4042



function onStartup(self)
	--print ("starting up")

end

-- When the Quick Build is completed, send a message to the zone script
function onRebuildNotifyState(self, msg)

    --print (msg.iState) 
        
    if (msg.iState == 2) then
        --self:GetObjectsInGroup{ group = "Stage_Group" }.objects:NotifyObject{objIDSender=self, name="RebuildComplete", param1=self:GetLOT().objtemplate}
        StunAndPlay(self, msg.player, self:GetLOT().objtemplate)
        print ("sending Stun & Play message")
    end

end



-- Remove player control and play the appropriate animation
function StunAndPlay(self, player, instrumentLOT)
    
    --local player2 = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    player:PlayCinematic { pathName = "Concert_Cam" }                                 
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "ToolTipTimer",self )
    
    local AnimationTime = ""
    local AnimationName = ""
    local SoundName = ""
    
    if instrumentLOT == CONSTANTS["GUITAR_LOT"] then
        
        AnimationName = "chicken"
        AnimationTime = player:GetAnimationTime{animationID=AnimationName}.time
        SoundName = "cow"
            
    elseif instrumentLOT == CONSTANTS["BASS_LOT"] then
        
        AnimationName = "headspin"
        AnimationTime = player:GetAnimationTime{animationID=AnimationName}.time
        SoundName = "digital_sign"
        
    elseif instrumentLOT == CONSTANTS["KEYBOARD_LOT"] then
        
        AnimationName = "wave"
        AnimationTime = player:GetAnimationTime{animationID=AnimationName}.time
        SoundName = "water_cooler_gubgub"
    
    elseif instrumentLOT == CONSTANTS["DRUM_LOT"] then
        
        AnimationName = "biglove"
        AnimationTime = player:GetAnimationTime{animationID=AnimationName}.time    
        SoundName = "grumpyidle_01m"
     
        
    end
        
        player:SetUserCtrlCompPause{bPaused = true}
        player:PlayAnimation{animationID = AnimationName, fPriority = 2.0}
        --print (AnimationName)
        player:PlaySound{strSoundName = SoundName}
       
        GAMEOBJ:GetTimer():AddTimerWithCancel( AnimationTime, "UnPausePlayer",self ) 


end



function onTimerDone(self, msg)

    if msg.name == "UnPausePlayer" then
        
        -- Get the player again (local variables aren't shared by different functions) and return player control
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player:EndCinematic{leadOut = 1.0}
        
        player:SetUserCtrlCompPause{bPaused = false}
    
    --Tool Tip must play later because Rebuild Code automatically closes any tool tips.
    elseif msg.name == "ToolTipTimer" then
        local player2 = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
        player2:DisplayTooltip { bShow = true, strText = Localize("AG_TOOLTIP_CON_ROCKSTAR"), iTime = 5000 }
        --print ("ToolTip Timer has FIRED " .. player2:GetID())
    end
    
end
