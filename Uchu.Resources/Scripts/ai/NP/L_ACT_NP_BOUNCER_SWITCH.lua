require('o_mis')

function onStartup(self)

end

function onUse(self, msg)
	local bouncer = getParent(self)
	if bouncer and bouncer:GetID() ~= "0" then 
		bouncer:NotifyObject{name = "switchPressed"}
	else
		print("Bouncer object has no parent!")
	end
end