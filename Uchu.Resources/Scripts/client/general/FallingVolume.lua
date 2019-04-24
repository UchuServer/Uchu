
function onCollisionPhantom(self, msg)

	local config = { {"objectID", "|" .. msg.objectID:GetID()}, {"leadIn", 1}, {"leadOut", 1}, {"lag", 0.1}, {"lockPos", true} }

	msg.objectID:AddCameraEffect{ effectType = "lookAt", effectID = "lookatFall", duration = 5, configData = config }
end