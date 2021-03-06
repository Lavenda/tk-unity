from sg_client import GetUnityEngine

import sgtk

import glob
import os

HookBaseClass = sgtk.get_hook_baseclass()


class UnitySessionCollector(HookBaseClass):
    """
    Collector that operates on the Unity session. Should inherit from the basic
    collector hook.
    """
        
    # Public interface
    def process_current_session(self, settings, parent_item):
        """
        Analyzes the current session open in Unity and parents a subtree of
        items under the parent_item passed in.

        :param dict settings: Configured settings for this collector
        :param parent_item: Root item instance

        """
        unity_engine = GetUnityEngine()
        session_item = parent_item.create_item(
            "unity.session",
            "Unity Session",
            unity_engine.Application.productName
        )
        session_item.properties["project_root"] = unity_engine.Application.dataPath
        import tempfile
        self._collect_file(
            session_item,
            os.path.join(tempfile.gettempdir(), unity_engine.Application.productName + ".mp4")
        )
        
    # Private interface
    def _collect_file(self, parent_item, path):
        """
        Process the supplied file path.

        :param parent_item: parent item instance
        :param path: Path to analyze
        :returns: The item that was created
        """

        # make sure the path is normalized. no trailing separator, separators
        # are appropriate for the current os, no double separators, etc.
        path = sgtk.util.ShotgunPath.normalize(path)
        
        # Users need to use the "Publish Recording..." menu item, not publish directly.
        # Otherwise the .mp4 file will not exist in the expected location
        if not os.path.isfile(path):
            self.logger.error('Could not find movie file at {}.'.format(path))
            self.logger.error('Please use the "Record Timeline" menu item to record your videos (see documentation for more details)')
            return

        publisher = self.parent

        # get info for the extension
        item_info = self._get_item_info(path)
        type_display = item_info["type_display"]

        display_name = publisher.util.get_publish_name(
            path, sequence=False)

        # create and populate the item
        file_item = parent_item.create_item(
            "unity.video", type_display, "Session Recording")
        file_item.set_icon_from_path(item_info["icon_path"])

        # all we know about the file is its path. set the path in its
        # properties for the plugins to use for processing.
        file_item.properties["path"] = path

        self.logger.info("Collected file: %s" % (path,))

        return file_item    
