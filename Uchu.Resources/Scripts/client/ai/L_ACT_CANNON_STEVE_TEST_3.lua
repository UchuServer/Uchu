--L_ACT_CANNON.lua
-- Client Side

-- global vars
-------------------------------
local platformTemplateID = 1863


function onStartup(self)
    -- Spawn the platform
--	local mypos = self:GetPosition().pos
--       RESMGR:LoadObject {
--			objectTemplate = platformTemplateID,
--			x = mypos.x,
--			y = mypos.y,
--			z = mypos.z,
--			rw = 1,
--			owner = self
--		}
		
	-- Load client side parameters Here
end

-- For the cannon, we rotate the object to face the aim position
function onShootingGalleryClientAimUpdate(self, msg)
-- do nothing
end
