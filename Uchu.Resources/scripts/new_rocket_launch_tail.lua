require('o_mis')


function onStartup(self)
--    print ("NRLT startup " )

    local targetZone = self:GetVar("targetZone")
    local targetScene = self:GetVar("targetScene")
    if targetScene == nil then self:SetVar("targetScene",""); targetScene = "" end
    local playSummary = self:GetVar("playSummary")
    local summaryCamera = self:GetVar("summaryCamera")
    if summaryCamera == nil then self:SetVar("summaryCamera",""); summaryCamera = "" end
    local launchPath = self:GetVar("launchCamera")
    if launchPath == nil then self:SetVar("launchCamera",""); launchPath = "" end
    local gmlevel = self:GetVar("GMLevel")
    if gmlevel == nil then gmlevel = 0 end

    local playerAnim = self:GetVar("playerAnim")
    local rocketAnim = self:GetVar("rocketAnim")
    if playerAnim == nil then self:SetVar("playerAnim","rocket-launch-AG"); playerAnim = self:GetVar("playerAnim") end
    if rocketAnim == nil then self:SetVar("rocketAnim","launch-AG"); rocketAnim = self:GetVar("rocketAnim") end

    local player=getObjectByName(self,"player")
	local rocket=getObjectByName(self,"rocket")
	
--	print ("player " .. player:GetID() .. " rocket " .. rocket:GetID())

    if GAMEOBJ:GetLocalCharID() == player:GetID()  then
   	   GAMEOBJ:GetTimer():AddTimerWithCancel( 50, "TimeExpired",self )
   	end

    if (player:GetGMLevel{}.ucGMLevel < gmlevel) then
    	player:DisplayTooltip { bShow = true, strText = "Launchpad under construction. Cannot launch now.", iTime = 5000 }
    	return
    end
    
    self:SetVar("preloads",0)
    player:PreloadAnimation{animationID = playerAnim, respondObjID = self  }

    if rocket:Exists() then
        rocket:PreloadAnimation{animationID = rocketAnim, respondObjID = self  }
    end
    
--    print ("Launching New Rocket to zone " .. targetZone .. ":" .. targetScene
--           .. " summary:" .. tostring(playSummary) .. " summaryCamera " .. summaryCamera
--           .. " launchCamera " .. launchPath
--           ..  " playerAnim " .. playerAnim .. " rocketAnim " .. rocketAnim
--          )
--    print ("Rocket Exists " .. tostring(rocket:Exists()))


	if GAMEOBJ:GetLocalCharID() == player:GetID() then
--	    print ("pausing controls")
        player:SetUserCtrlCompPause{bPaused = true}
    end
    
    if rocket:Exists() then
--	    print ("waiting for just a few seconds")
        rocket:SetCustomBuild{}
       	GAMEOBJ:GetTimer():AddTimerWithCancel( 0.5, "EquippedRocket",self )
        player:PlayCinematic { pathName = self:GetVar("launchCamera") }
    else
--	    print ("Waiting for rocket to load")
        self:SendLuaNotificationRequest{requestTarget=player, messageName="ChildRenderComponentReady"}
    end
    
    player:EquipInventory { itemtoequip = rocket }
   	
end

function onAnimationFinishedPreloading(self,msg)
    local preloads = self:GetVar("preloads")
    preloads = preloads+1
    self:SetVar("preloads",preloads)
    --print ("preloads " .. preloads)
end


function notifyChildRenderComponentReady(self,other,msg)
--    print ("notifyChildRenderComponentReady " .. tostring(self) .. " " .. tostring(other) .. " " .. tostring(msg) )
    local player=getObjectByName(self,"player")
	local rocket=getObjectByName(self,"rocket")
	rocket:RefreshProxy()

    rocket:PreloadAnimation{animationID = rocketAnim, respondObjID = self  }

	self:SendLuaNotificationCancel{requestTarget=player, messageName="ChildRenderComponentReady"}

--    print ("Rocket Ready")
--    print ("waiting for just a few seconds more")
   	GAMEOBJ:GetTimer():AddTimerWithCancel( .5, "EquippedRocket",self )
    player:PlayCinematic { pathName = self:GetVar("launchCamera") }
end



function BeginTheDance(self)
    local player=getObjectByName(self,"player")
	local rocket=getObjectByName(self,"rocket")

    if self:GetVar("preloads")<2 then
   	    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "EquippedRocket",self )
    end
    
--    print ("Begin The Dance")

    rocket:SetCustomBuild{}
	rocket:MakePhysics {}
	rocket:DetachObject {} 
	rocket:SetOffscreenAnimation { bAnimateOffscreen = true }
    player:PlayAnimation {animationID = "setdown-carry"}
	player:SetAnimationSet {strSet = "", bPush=true }
	
    local lpos = self:GetPosition{}
--    print ("setting pos to " .. lpos.pos.x .. "," .. lpos.pos.y .. "," .. lpos.pos.z)
    rocket:SetPosition { pos = lpos.pos }
    local rot = self:GetRotation{}
--    print ("setting rot to " .. rot.x .. "," .. rot.y .. "," .. rot.z .. "," .. rot.w)
    rocket:SetRotation {x = rot.x, y = rot.y, z = rot.z, w = rot.w }
    rocket:SetVisible {visible = true }     

	GAMEOBJ:GetTimer():AddTimerWithCancel( 3.0,  "PlayerAnimate",  self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( 5.1,  "RocketLaunch",   self )
	GAMEOBJ:GetTimer():AddTimerWithCancel( 7.06, "RocketFiring",   self )

end

function onTimerDone (self,msg)
--    print ("timerDone..." .. msg.name)
    local player = getObjectByName(self, "player")
    local rocket = getObjectByName(self, "rocket")
    local rocketPosition = self:GetPosition{}.pos
    local rocketRotation = self:GetRotation{}
    local playerAnim = self:GetVar("playerAnim")
    local rocketAnim = self:GetVar("rocketAnim")


    if (msg.name == "EquippedRocket") then
        BeginTheDance(self)
    end


	if (msg.name == "PlayerAnimate") then
        if(player:GetID() == GAMEOBJ:GetLocalCharID() ) then
            player:SetPosition {pos = rocketPosition}
            player:SetRotation {x = rocketRotation.x, y = rocketRotation.y, z = rocketRotation.z, w = rocketRotation.w}
        end

        player:PlayAnimation{animationID = playerAnim }
		rocket:PlayAnimation{animationID = rocketAnim }

        -- Set zone player timer
		local zTime = rocket:GetAnimationTime{animationID = rocketAnim }.time
        if (zTime == 0) then zTime = 4.0 end
 		GAMEOBJ:GetTimer():AddTimerWithCancel( zTime, "ZoneTimer",self )

    end

    if (msg.name == "RocketLaunch") then
		rocket:PlayFXEffect{effectType = "launch"}
    end

	if (msg.name == "RocketFiring") then
		rocket:PlayFXEffect{effectType = "firing"}
		
    end

    if ( msg.name == "ZoneTimer" ) then
        rocket:SetVisible {visible = false, fadeTime=0 }
        player:SetVisible {visible = false, fadeTime=0 }
        if GAMEOBJ:GetLocalCharID() == player:GetID()  then
            rocket:PlayAnimation{ animationID = "idle" }
            player:SetAnimationSet { strSet = "", bPush=false }
            player:UnEquipInventory { itemtounequip = rocket }
            player:SetFlag{ iFlagID = 32, bFlag = false }   -- flag 32 is play the rocket landing or not.  false means play


           GAMEOBJ:GetTimer():CancelTimer("TimeExpired",self)
           self:GetParentObj{}.objIDParent:FireEventServerSide { senderID = player, args = "ClearLauncher" }

            if (self:GetVar("playSummary")) then
                player:DisplayZoneSummary{ sender=self, isZoneStart=false }
                player:PlayCinematic { pathName = self:GetVar("summaryCamera") }
            else
                 self:GetParentObj{}.objIDParent:FireEventServerSide { senderID = player, args = "ZonePlayer"}
            end
--        else print ("Foriegn rocket launch done")            
        end
    end
    
    if msg.name == "TimeExpired" and GAMEOBJ:GetLocalCharID() == player:GetID() then
            self:GetParentObj{}.objIDParent:FireEventServerSide{senderID = player, args = "ZonePlayer"}
    end
    
end

function onZoneSummaryDismissed (self,msg)
    local player = getObjectByName(self, "player")
    
--    print ("zoneSummaryDismissed " .. player:GetID() .. " " .. GAMEOBJ:GetLocalCharID() )

    if GAMEOBJ:GetLocalCharID() == player:GetID()  then
       self:GetParentObj{}.objIDParent:FireEventServerSide{senderID = player, args = "ZonePlayer"}
--    else   print ("Foriegn ZoneSummaryDismissed")
    end
end
