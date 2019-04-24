--------------------------------------------------------------
-- Server Constants/Settings for Avant Gardens Scripts
--
-- We need this because the server cannot use the other 
-- constants file due to "Localize" calls
--------------------------------------------------------------

CONSTANTS = {}
CONSTANTS["COURSE_MANAGER_GROUP"] = "race"
CONSTANTS["COURSE_MANAGER_LOT"] = 6011 
CONSTANTS["COUNTDOWN_DELAY_SEC"] = 3

--------------------------------------------------------------
-- Locates the course manager object in the course manager
-- group.
--------------------------------------------------------------
function GetCourseManager(self)

    -- get all objects in course manager group
    local objects = self:GetObjectsInGroup{ group = CONSTANTS["COURSE_MANAGER_GROUP"] }.objects

    -- loop through objects and return the first object that matches the 
    -- course manager lot
    for i = 1, table.maxn (objects) do 
        if (objects[i]:GetLOT().objtemplate == CONSTANTS["COURSE_MANAGER_LOT"]) then
            return objects[i]     
        end
    end

    return nil  

end



