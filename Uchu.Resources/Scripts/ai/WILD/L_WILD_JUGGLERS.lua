--require('State')
--require('o_StateCreate')
--require('o_mis')
--require('o_Main')

function onStartup(self) 

    self:AddObjectToGroup{ group = "jug1" }

end

function onUse(self, msg)

    local friends = self:GetObjectsInGroup{ group = "jug1" }.objects
    for i = 1, table.maxn (friends) do      
        if friends[i]:GetLOT().objtemplate == 7318 then
             friends[i]:NotifyObject{ name="Mimey" }
        end
    end

end

function onNotifyObject(self, msg)

    if msg.name == "Juggly" then
        self:PlayAnimation{ animationID = "cast" }
        print "Juggle time!"
    end

end