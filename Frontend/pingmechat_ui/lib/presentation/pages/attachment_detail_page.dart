import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';
import 'package:video_player/video_player.dart';

import '../../domain/models/attachment.dart';
import '../widgets/message_widget.dart'; // Import the video_player package.

class AttachmentDetailView extends StatefulWidget {
  final Attachment attachment;

  const AttachmentDetailView({Key? key, required this.attachment})
      : super(key: key);

  @override
  _AttachmentDetailViewState createState() => _AttachmentDetailViewState();
}

class _AttachmentDetailViewState extends State<AttachmentDetailView> {
  VideoPlayerController? _videoPlayerController;
  Future<void>? _initializeVideoPlayerFuture;

  @override
  void initState() {
    super.initState();
    if (widget.attachment.fileType == 'Video') {
      _videoPlayerController =
          VideoPlayerController.network(widget.attachment.fileUrl);
      _initializeVideoPlayerFuture = _videoPlayerController!.initialize();
    }
  }

  @override
  void dispose() {
    _videoPlayerController?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return _getAttachmentDetailView(widget.attachment);
  }

  Widget _getAttachmentDetailView(Attachment attachment) {
    switch (attachment.fileType) {
      case 'Image':
        return Center(
          child: CachedNetworkImage(
            imageUrl: attachment.fileUrl,
            placeholder: (context, url) => CircularProgressIndicator(),
            errorWidget: (context, url, error) => Icon(Icons.error),
          ),
        );
      case 'Video':
        if (_videoPlayerController != null) {
          return FutureBuilder(
            future: _initializeVideoPlayerFuture,
            builder: (context, snapshot) {
              if (snapshot.connectionState == ConnectionState.done) {
                return Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min, // Keep the controls compact
                    children: [
                      AspectRatio(
                        aspectRatio: _videoPlayerController!.value.aspectRatio,
                        child: VideoPlayer(_videoPlayerController!),
                      ),
                      VideoProgressIndicator(
                        _videoPlayerController!,
                        allowScrubbing: true,
                      ),
                      ElevatedButton(
                        onPressed: () {
                          setState(() {
                            _videoPlayerController!.value.isPlaying
                                ? _videoPlayerController!.pause()
                                : _videoPlayerController!.play();
                          });
                        },
                        child: Icon(
                          _videoPlayerController!.value.isPlaying
                              ? Icons.pause
                              : Icons.play_arrow,
                        ),
                      ),
                    ],
                  ),
                );
              } else if (snapshot.hasError) {
                return Center(child: Text('Error loading video'));
              } else {
                return Center(child: CircularProgressIndicator());
              }
            },
          );
        } else {
          return Center(child: Text('Video URL not valid'));
        }
      case 'Audio':
        return Center(
            child: AudioPlayerWidget(
                url: attachment
                    .fileUrl)); // Assuming AudioPlayerWidget is defined elsewhere.
      case 'File':
        return Center(child: Text('File preview is not supported.'));
      default:
        return Center(child: Text('Attachment preview is not supported.'));
    }
  }
}
