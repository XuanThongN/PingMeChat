import 'package:flutter/material.dart';
import 'package:pingmechat_ui/presentation/widgets/custom_circle_avatar.dart';

class GroupCallPage extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          // Background Image
          Container(
            decoration: BoxDecoration(
              image: DecorationImage(
                image: NetworkImage(
                    'https://via.placeholder.com/800x1600'), // Replace with the background image URL
                fit: BoxFit.cover,
              ),
            ),
          ),
          // Overlay Content
          SafeArea(
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Meeting Title and Organizer Info
                  Text(
                    'Meeting with Lora Adom',
                    style: TextStyle(
                      color: Colors.white,
                      fontSize: 32,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  SizedBox(height: 10),
                  Row(
                    children: [
                      CustomCircleAvatar(
                        backgroundImage: NetworkImage(
                            'https://via.placeholder.com/150'), // Replace with organizer's image URL
                        radius: 25,
                      ),
                      SizedBox(width: 10),
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Lora Adom',
                            style: TextStyle(
                              color: Colors.white,
                              fontSize: 16,
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          Text(
                            'Meeting organizer',
                            style: TextStyle(
                              color: Colors.white.withOpacity(0.7),
                              fontSize: 14,
                            ),
                          ),
                        ],
                      ),
                    ],
                  ),
                  Spacer(),
                  // Chat Messages
                  _buildChatMessages(),
                  Spacer(),
                  // Call Control Buttons
                  _buildCallControlButtons(),
                  SizedBox(height: 20),
                  // Participants Row
                  _buildParticipantsRow(),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildChatMessages() {
    List<Map<String, String>> messages = [
      {
        'avatarUrl':
            'https://via.placeholder.com/150', // Replace with avatar URL
        'name': 'Dean Ronload',
        'message': 'Sounds reasonable',
      },
      {
        'avatarUrl':
            'https://via.placeholder.com/150', // Replace with avatar URL
        'name': 'Annei Ellison',
        'message': 'What about our profit?',
      },
      {
        'avatarUrl':
            'https://via.placeholder.com/150', // Replace with avatar URL
        'name': 'John Borino',
        'message': 'What led you to this thought?',
      },
    ];

    return Column(
      children: messages.map((msg) {
        return ListTile(
          leading: CircleAvatar(
            backgroundImage: NetworkImage(msg['avatarUrl']!),
          ),
          title: Text(
            msg['name']!,
            style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold),
          ),
          subtitle: Text(
            msg['message']!,
            style: TextStyle(color: Colors.white.withOpacity(0.8)),
          ),
        );
      }).toList(),
    );
  }

  Widget _buildCallControlButtons() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceEvenly,
      children: [
        _buildControlButton(Icons.mic, 'Mute'),
        _buildControlButton(Icons.volume_up, 'Speaker'),
        _buildControlButton(Icons.videocam, 'Video'),
        _buildControlButton(Icons.chat, 'Chat', buttonColor: Colors.green),
        _buildControlButton(Icons.call_end, 'End', buttonColor: Colors.red),
      ],
    );
  }

  Widget _buildControlButton(IconData icon, String label,
      {Color buttonColor = Colors.white}) {
    return Column(
      children: [
        CircleAvatar(
          backgroundColor: buttonColor,
          radius: 30,
          child: Icon(icon, color: Colors.black, size: 28),
        ),
        SizedBox(height: 5),
        Text(
          label,
          style: TextStyle(color: Colors.white),
        ),
      ],
    );
  }

  Widget _buildParticipantsRow() {
    List<String> participantImages = [
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
      'https://via.placeholder.com/150', // Replace with participant's image URL
    ];

    return SingleChildScrollView(
      scrollDirection: Axis.horizontal,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: participantImages.map((url) {
          return Padding(
            padding: const EdgeInsets.symmetric(horizontal: 8.0),
            child: Stack(
              children: [
                CircleAvatar(
                  backgroundImage: NetworkImage(url),
                  radius: 30,
                ),
                Positioned(
                  bottom: 0,
                  right: 0,
                  child: CircleAvatar(
                    backgroundColor: Colors.white,
                    radius: 10,
                    child: Icon(Icons.mic_off, size: 15, color: Colors.black),
                  ),
                ),
              ],
            ),
          );
        }).toList(),
      ),
    );
  }
}
