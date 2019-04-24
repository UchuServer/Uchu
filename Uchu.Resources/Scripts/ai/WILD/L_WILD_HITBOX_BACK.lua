require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

function onStartup(self)

    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.2, "HitboxStartup",self )
    GAMEOBJ:GetTimer():AddTimerWithCancel( 0.3, "HitboxFollow",self )

end

function onOnHit(self, msg)

    local snakes = self:GetObjectsInGroup{ group = "hitboxes" }.objects

    for i = 1, table.maxn (snakes) do
        snakes[i]:NotifyObject{name = "hit", ObjIDSender = self}
    end

end

function onTimerDone(self, msg)

	if msg.name == "HitboxStartup" then
        self:AddObjectToGroup{ group = "hitboxes" }
        self:OverrideFriction{bEnableOverride = true, fFriction = 40}

    elseif msg.name == "HitboxFollow" then
        local friends = self:GetObjectsInGroup{ group = "hitboxes" }.objects

        for i = 1, table.maxn (friends) do
            if friends[i]:GetLOT().objtemplate == 6623 then
                self:FollowTarget { targetID = friends[i], radius = 3, speed = 6, keepFollowing = true }
            end
        end
    end

end