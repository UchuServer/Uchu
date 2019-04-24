function onStartup(self)
    
    self:SetVar("isCoughing",1)
    self:PlayFXEffect{ name  = "Burn", effectID = 295, effectType = "running"}
    self:SetProximityRadius{radius = 10}

end

function onFireEventServerSide(self, msg)  
  
    if msg.args == 'physicsReady' then
         self:PlayFXEffect{ name  = "Burn", effectID = 295, effectType = "running"}  
    end

end

function onCollisionPhantom(self, msg)

    local target = msg.senderID
	local faction = target:GetFaction()

    if self:GetVar("isCoughing") == 1 and faction.faction == 1 then
        target:PlayAnimation{animationID = "cough"}
        self:SetVar("isCoughing",0)
        GAMEOBJ:GetTimer():AddTimerWithCancel( 1.0, "CoughAgain", self )
    end

end

function onTimerDone(self, msg)
	if (msg.name == "CoughAgain") then
        self:SetVar("isCoughing",1)
    end
end