require('o_mis')

local bCINEMA_ONCE = false

function onStartup(self, msg)
    print "preloading"
    local Frisbeed = self:GetObjectsInGroup{ group = "Frisbee"}.objects

    for i = 1, table.maxn (Frisbeed) do
        if Frisbeed[i]:GetLOT().objtemplate == 7711 then
            Frisbeed[i]:PreloadAnimation{animationID = "launch", respondObjID = self}
        end
    end

end

function onClientUse(self, msg)
print "Use"
    local player = msg.user
    player:PlayCinematic { pathName = "LandingCine_0" }

    local Frisbeed = self:GetObjectsInGroup{ group = "Frisbee"}.objects

    for i = 1, table.maxn (Frisbeed) do
        if Frisbeed[i]:GetLOT().objtemplate == 7711 then
                Frisbeed[i]:PlayAnimation{animationID = "launch"}
                Frisbeed[i]:SetOffscreenAnimation{bAnimateOffscreen = true}
        end
    end

end

function onGetOverridePickType(self, msg)

	msg.ePickType = 14
	return msg

end