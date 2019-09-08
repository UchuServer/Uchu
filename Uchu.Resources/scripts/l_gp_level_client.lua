require('o_mis')
function onStartup(self) 

    self:AddObjectToGroup{ group = "Level" }

end
    
function onNotifyObject(self, msg)

    if msg.name == "Raise" then
        --print "Lower"
        self:PlayAnimation{animationID = "raise"}

    elseif msg.name == "Lower" then
        --print "Lower"
        self:PlayAnimation{animationID = "lower"}
    end
        
end
