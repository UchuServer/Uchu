require('o_mis')

function onCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()

    if faction and faction.faction == 1 then
        target:SetAnimationSet{strSet = "swim"}
        print ("Swim "..target:GetName().name )
    end

end

function onOffCollisionPhantom(self, msg)

    local target = msg.objectID
	local faction = target:GetFaction()

    if faction and faction.faction == 1 then
        target:SetAnimationSet{strSet = ""}
        print ("Walk "..target:GetName().name )
    end
end
