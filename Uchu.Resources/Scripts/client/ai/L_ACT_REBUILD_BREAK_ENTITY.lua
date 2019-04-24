require('o_mis')
--///////////////////////////////////////////////////////////////////////////////////////
--//            Generic Rebuild -- Script (CLIENT)
--//   - The spawned entity that will be breaking a rebuild
--///////////////////////////////////////////////////////////////////////////////////////

    
function onRenderComponentReady(self, msg) 
		-- play the rebuild enter animation on the client
		local anim_time = self:GetAnimationTime{  animationID = "rebuild-enter" }.time
		self:PlayFXEffect{effectType = "rebuild-enter"}
end
