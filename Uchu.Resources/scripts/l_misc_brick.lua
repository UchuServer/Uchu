require('Vector')

function onCollision(self, msg)
	local target = msg.objectID

--If I'm facing the opposite direction (180) and I'm below the object then ignore collision?
        local myPos = Vector.new(self:GetPosition().pos)
        local hisPos = Vector.new(target:GetPosition().pos)
        local dist = hisPos - myPos;
        -- If the colider is lower than I am
        if (dist.y <= 2) then
					local myHeading = getHeading(self);
					local hisHeading = getHeading(target);
					-- Dot product these?
					local dot = Vector.__mul(myHeading, hisHeading);
				
				--print (dot)
					if (dot < 0) then
						msg.ignoreCollision = true
						return msg
					end 	
      end
end
	

function getHeading(obj)
    local q = obj:GetRotation()
        return({
                x = 2 * q.y * q.w + 2 * q.x * q.x,
                y = 2 * q.z * q.y - 2 * q.x * q.w,
                z = q.z*q.z + q.w*q.w - q.x*q.x - q.y*q.y,
            })

end
        
