--require('State')
--require('o_StateCreate')
--require('o_mis')
--require('o_Main')

function onStartup(self) 

    self:SetVar("resetme", "on")
    self:SetVar("home", self:GetPosition().pos )
    self:AddObjectToGroup{ group = "jug1" }

end

function onArrived(self, msg)

	if (msg.isLastPoint == true) then
        GAMEOBJ:GetTimer():AddTimerWithCancel( 2.0, "Juggle",self )
        GAMEOBJ:GetTimer():AddTimerWithCancel( 16.2, "RestorePath",self )
    end
	
end

function onNotifyObject(self, msg)

    if msg.name == "Mimey" then
        if self:GetVar("resetme") == "on" then
            self:SetVar("resetme", "off")
            self:SetVar("attached_path", "Mime_Path")
            self:FollowWaypoints()
            self:SetPathingSpeed{ speed = 0.6 } 
        end
   end

end

function onTimerDone(self, msg)

	if msg.name == "Juggle" then
        print "Juggle"
        self:SetRotation{x=0, y=1, z=0, w=0}
        self:PlayAnimation{ animationID = "cast" }
        local friends = self:GetObjectsInGroup{ group = "jug1" }.objects
        for i = 1, table.maxn (friends) do      
            if friends[i]:GetLOT().objtemplate ~= 7318 then
               friends[i]:NotifyObject{ name="Juggly" }
            end
        end

    elseif msg.name == "RestorePath" then
        print "Go home"
        self:SetVar("resetme", "on")
        self:GoTo{speed = 0.6, target = self:GetVar("home")}
    end

end