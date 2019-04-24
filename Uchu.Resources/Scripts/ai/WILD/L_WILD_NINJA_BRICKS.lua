function onStartup(self, msg)

    self:AddObjectToGroup{ group = "Ninjastuff" }

end

function onNotifyObject(self, msg)

    if msg.name == "Crane" then
        self:PlayAnimation{animationID = "crane"}

    elseif msg.name == "Tiger" then
        self:PlayAnimation{animationID = "tiger"}

    elseif msg.name == "Mantis" then
        self:PlayAnimation{animationID = "mantis"}
    end

end