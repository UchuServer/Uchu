function onStartup(self, msg)

    self:AddObjectToGroup{ group = "Ninjastuff" }
    self:PlayAnimation{ animationID = "bow" }

end

function onNotifyObject(self, msg)

    if msg.name == "Crane" then
        self:PlayAnimation{animationID = "crane"}

    elseif msg.name == "Tiger" then
        self:PlayAnimation{animationID = "tiger"}

    elseif msg.name == "Mantis" then
        self:PlayAnimation{animationID = "mantis"}

    elseif msg.name == "Bow" then
        self:PlayAnimation{animationID = "bow"}
    end

end