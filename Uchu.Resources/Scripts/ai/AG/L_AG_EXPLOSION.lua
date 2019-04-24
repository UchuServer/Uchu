function onStartup(self, msg)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 10.0, "Asplode",self )
    --self:PlayFXEffect{name = "kaboom", effectType = "boom"}
    GAMEOBJ:GetTimer():AddTimerWithCancel( 1.7, "BOOMIES",self )
    self:SetVar("posx", self:GetPosition().pos.x )
    self:SetVar("posy", self:GetPosition().pos.y )
    self:SetVar("posz", self:GetPosition().pos.z )
    self:SetVar("rotx", self:GetRotation{}.x)
    self:SetVar("roty", self:GetRotation{}.y)
    self:SetVar("rotz", self:GetRotation{}.z)
    self:SetVar("rotw", self:GetRotation{}.w)

end

function onTimerDone(self, msg)

	if msg.name == "Asplode" then
        local superran = math.random(-20,20)
        local timerran = math.random(0,10)

        self:SetPosition{pos = {x = self:GetVar("posx") + superran, y = self:GetVar("posy"), z = self:GetVar("posz") + superran}}
        self:SetRotation{x=self:GetVar("rotx"), y=self:GetVar("roty"), z=self:GetVar("rotz"), w=self:GetVar("rotw")}
        GAMEOBJ:GetTimer():AddTimerWithCancel( (10.0 + timerran), "Asplode",self )
       -- GAMEOBJ:GetTimer():AddTimerWithCancel( 1.7, "BOOMIES",self )
        self:CastSkill{skillID = 318}       
       --self:PlayFXEffect{name = "kaboom", effectType = "boom"}

        
  --  elseif msg.name == "BOOMIES" then
       
    end

end