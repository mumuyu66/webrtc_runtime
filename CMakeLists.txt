cmake_minimum_required(VERSION 2.8)

project(webrtc_runtime)
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++11")
set(CMAKE_INSTALL_RPATH_USE_LINK_PATH TRUE)

if (WIN32)
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static -static-libgcc -static-libstdc++")
endif ()

add_definitions("-DWEBRTC_WIN -DWEBRTC_NS_FLOAT -DUNICODE -D_UNICODE -D_GNU_SOURCE")
#-DWEBRTC_UNTRUSTED_DELAY

include_directories(
    ${CMAKE_CURRENT_SOURCE_DIR}
	${CMAKE_CURRENT_SOURCE_DIR}/codec
	${CMAKE_CURRENT_SOURCE_DIR}/webrtc/common_audio/signal_processing/include
	${CMAKE_CURRENT_SOURCE_DIR}/webrtc/modules/audio_coding/codecs/isac/main/include
)

set(WEBRTC_SRC_

common_audio/channel_buffer.cc
common_audio/sparse_fir_filter.cc
common_audio/ring_buffer.c
common_audio/real_fourier_ooura.cc
common_audio/vad/vad.cc
common_audio/vad/vad_filterbank.c
common_audio/vad/vad_gmm.c
common_audio/vad/vad_sp.c
common_audio/vad/vad_core.c
common_audio/vad/webrtc_vad.c
common_audio/audio_ring_buffer.cc
common_audio/fft4g.c
common_audio/blocker.cc
common_audio/wav_header.cc
common_audio/audio_converter.cc
common_audio/audio_util.cc
common_audio/wav_file.cc
common_audio/smoothing_filter.cc
common_audio/lapped_transform.cc
common_audio/real_fourier.cc
common_audio/window_generator.cc
common_audio/fir_filter.cc

common_audio/fir_filter_sse.cc
common_audio/resampler/sinc_resampler_sse.cc

common_audio/resampler/push_sinc_resampler.cc
common_audio/resampler/sinc_resampler.cc
common_audio/resampler/push_resampler.cc
common_audio/resampler/sinusoidal_linear_chirp_source.cc
common_audio/resampler/resampler.cc
common_audio/signal_processing/complex_fft.c
common_audio/signal_processing/resample_48khz.c
common_audio/signal_processing/resample_by_2_internal.c
common_audio/signal_processing/levinson_durbin.c
common_audio/signal_processing/dot_product_with_scale.cc
common_audio/signal_processing/cross_correlation.c
common_audio/signal_processing/filter_ar_fast_q12.c
common_audio/signal_processing/vector_scaling_operations.c
common_audio/signal_processing/downsample_fast.c
common_audio/signal_processing/refl_coef_to_lpc.c
common_audio/signal_processing/ilbc_specific_functions.c
common_audio/signal_processing/real_fft.c
common_audio/signal_processing/resample.c
common_audio/signal_processing/spl_sqrt.c
common_audio/signal_processing/auto_correlation.c
common_audio/signal_processing/spl_init.c
common_audio/signal_processing/division_operations.c
common_audio/signal_processing/get_scaling_square.c
common_audio/signal_processing/min_max_operations.c
common_audio/signal_processing/spl_sqrt_floor.c
common_audio/signal_processing/auto_corr_to_refl_coef.c
common_audio/signal_processing/spl_inl.c
common_audio/signal_processing/filter_ar.c
common_audio/signal_processing/resample_by_2.c
common_audio/signal_processing/resample_fractional.c
common_audio/signal_processing/energy.c
common_audio/signal_processing/get_hanning_window.c
common_audio/signal_processing/sqrt_of_one_minus_x_squared.c
common_audio/signal_processing/randomization_functions.c
common_audio/signal_processing/complex_bit_reverse.c
common_audio/signal_processing/splitting_filter.c
common_audio/signal_processing/filter_ma_fast_q12.c
common_audio/signal_processing/copy_set_operations.c
common_audio/signal_processing/lpc_to_refl_coef.c
modules/audio_processing/typing_detection.cc
modules/audio_processing/vad/pole_zero_filter.cc
modules/audio_processing/vad/voice_activity_detector.cc
modules/audio_processing/vad/pitch_internal.cc
modules/audio_processing/vad/standalone_vad.cc
modules/audio_processing/vad/vad_circular_buffer.cc
modules/audio_processing/vad/gmm.cc
modules/audio_processing/vad/vad_audio_proc.cc
modules/audio_processing/vad/pitch_based_vad.cc
modules/audio_processing/rms_level.cc
modules/audio_processing/agc/utility.cc
modules/audio_processing/agc/agc.cc
modules/audio_processing/agc/loudness_histogram.cc
modules/audio_processing/agc/legacy/digital_agc.c
modules/audio_processing/agc/legacy/analog_agc.c
modules/audio_processing/agc/agc_manager_direct.cc
modules/audio_processing/beamformer/nonlinear_beamformer.cc
modules/audio_processing/beamformer/array_util.cc
modules/audio_processing/beamformer/covariance_matrix_generator.cc
modules/audio_processing/aecm/echo_control_mobile.cc
modules/audio_processing/aecm/aecm_core_c.cc
modules/audio_processing/aecm/aecm_core.cc
modules/audio_processing/aec/aec_core_sse2.cc
modules/audio_processing/aec/aec_resampler.cc
modules/audio_processing/aec/echo_cancellation.cc
modules/audio_processing/aec/aec_core.cc
modules/audio_processing/low_cut_filter.cc
modules/audio_processing/audio_buffer.cc
modules/audio_processing/transient/wpd_tree.cc
modules/audio_processing/transient/wpd_node.cc
modules/audio_processing/transient/moving_moments.cc
modules/audio_processing/transient/file_utils.cc
modules/audio_processing/transient/transient_detector.cc
modules/audio_processing/transient/transient_suppressor.cc
modules/audio_processing/splitting_filter.cc
modules/audio_processing/residual_echo_detector.cc
modules/audio_processing/echo_detector/moving_max.cc
modules/audio_processing/echo_detector/mean_variance_estimator.cc
modules/audio_processing/echo_detector/normalized_covariance_estimator.cc
modules/audio_processing/echo_detector/circular_buffer.cc
modules/audio_processing/logging/apm_data_dumper.cc
modules/audio_processing/ns/ns_core.c
modules/audio_processing/ns/nsx_core.c
modules/audio_processing/ns/noise_suppression.c
modules/audio_processing/ns/noise_suppression_x.c
modules/audio_processing/ns/nsx_core_c.c
modules/audio_processing/agc2/gain_controller2.cc
modules/audio_processing/agc2/digital_gain_applier.cc

modules/audio_processing/aec3/decimator_by_4.cc
modules/audio_processing/aec3/echo_canceller3.cc
modules/audio_processing/aec3/render_delay_buffer.cc
modules/audio_processing/aec3/echo_remover.cc
modules/audio_processing/aec3/frame_blocker.cc
modules/audio_processing/aec3/render_delay_controller.cc
modules/audio_processing/aec3/matched_filter.cc
modules/audio_processing/aec3/block_framer.cc
modules/audio_processing/aec3/block_processor.cc
modules/audio_processing/aec3/cascaded_biquad_filter.cc
modules/audio_processing/aec3/echo_path_delay_estimator.cc
modules/audio_processing/aec3/matched_filter_lag_aggregator.cc
modules/audio_processing/aec3/block_processor_metrics.cc
modules/audio_processing/aec3/downsampled_render_buffer.cc
modules/audio_processing/aec3/render_buffer.cc
modules/audio_processing/aec3/output_selector.cc
modules/audio_processing/aec3/suppression_filter.cc
modules/audio_processing/aec3/comfort_noise_generator.cc
modules/audio_processing/aec3/subtractor.cc
modules/audio_processing/aec3/render_signal_analyzer.cc
modules/audio_processing/aec3/aec_state.cc
modules/audio_processing/aec3/adaptive_fir_filter.cc
modules/audio_processing/aec3/aec3_common.cc
modules/audio_processing/aec3/aec3_fft.cc
modules/audio_processing/aec3/block_framer.cc
modules/audio_processing/aec3/erl_estimator.cc
modules/audio_processing/aec3/main_filter_update_gain.cc
modules/audio_processing/aec3/echo_remover_metrics.cc
modules/audio_processing/aec3/residual_echo_estimator.cc
modules/audio_processing/aec3/suppression_gain.cc
modules/audio_processing/aec3/erle_estimator.cc
modules/audio_processing/aec3/shadow_filter_update_gain.cc
modules/audio_processing/aec3/echo_path_variability.cc
modules/audio_processing/aec3/render_delay_controller_metrics.cc

modules/audio_processing/gain_control_for_experimental_agc.cc
modules/audio_processing/three_band_filter_bank.cc
modules/audio_processing/include/config.cc
modules/audio_processing/include/audio_processing.cc
modules/audio_processing/include/aec_dump.cc
modules/audio_processing/intelligibility/intelligibility_utils.cc
modules/audio_processing/intelligibility/intelligibility_enhancer.cc
modules/audio_processing/utility/ooura_fft_sse2.cc
modules/audio_processing/utility/delay_estimator_wrapper.cc
modules/audio_processing/utility/delay_estimator.cc
modules/audio_processing/utility/block_mean_calculator.cc
modules/audio_processing/utility/ooura_fft.cc
modules/audio_processing/level_controller/gain_applier.cc
modules/audio_processing/level_controller/saturating_gain_estimator.cc
modules/audio_processing/level_controller/gain_selector.cc
modules/audio_processing/level_controller/noise_level_estimator.cc
modules/audio_processing/level_controller/biquad_filter.cc
modules/audio_processing/level_controller/level_controller.cc
modules/audio_processing/level_controller/down_sampler.cc
modules/audio_processing/level_controller/noise_spectrum_estimator.cc
modules/audio_processing/level_controller/peak_level_estimator.cc
modules/audio_processing/level_controller/signal_classifier.cc

api/audio_codecs/audio_decoder.cc
api/audio_codecs/audio_encoder.cc
api/audio_codecs/audio_format.cc
modules/audio_coding/codecs/isac/locked_bandwidth_info.cc
modules/audio_coding/codecs/isac/main/source/arith_routines.c
modules/audio_coding/codecs/isac/main/source/arith_routines_hist.c
modules/audio_coding/codecs/isac/main/source/arith_routines_logist.c
modules/audio_coding/codecs/isac/main/source/audio_decoder_isac.cc
modules/audio_coding/codecs/isac/main/source/audio_encoder_isac.cc
modules/audio_coding/codecs/isac/main/source/bandwidth_estimator.c
modules/audio_coding/codecs/isac/main/source/crc.c
modules/audio_coding/codecs/isac/main/source/decode_bwe.c
modules/audio_coding/codecs/isac/main/source/decode.c
modules/audio_coding/codecs/isac/main/source/encode.c
modules/audio_coding/codecs/isac/main/source/encode_lpc_swb.c
modules/audio_coding/codecs/isac/main/source/entropy_coding.c
modules/audio_coding/codecs/isac/main/source/fft.c
modules/audio_coding/codecs/isac/main/source/filterbanks.c
modules/audio_coding/codecs/isac/main/source/filterbank_tables.c
modules/audio_coding/codecs/isac/main/source/filter_functions.c
modules/audio_coding/codecs/isac/main/source/intialize.c
modules/audio_coding/codecs/isac/main/source/isac.c
modules/audio_coding/codecs/isac/main/source/lattice.c
modules/audio_coding/codecs/isac/main/source/lpc_analysis.c
modules/audio_coding/codecs/isac/main/source/lpc_gain_swb_tables.c
modules/audio_coding/codecs/isac/main/source/lpc_shape_swb12_tables.c
modules/audio_coding/codecs/isac/main/source/lpc_shape_swb16_tables.c
modules/audio_coding/codecs/isac/main/source/lpc_tables.c
modules/audio_coding/codecs/isac/main/source/pitch_estimator.c
modules/audio_coding/codecs/isac/main/source/pitch_filter.c
modules/audio_coding/codecs/isac/main/source/pitch_gain_tables.c
modules/audio_coding/codecs/isac/main/source/pitch_lag_tables.c
modules/audio_coding/codecs/isac/main/source/spectrum_ar_model_tables.c
modules/audio_coding/codecs/isac/main/source/transform.c
modules/audio_coding/codecs/legacy_encoded_audio_frame.cc

system_wrappers/source/aligned_malloc.cc
system_wrappers/source/cpu_features.cc
system_wrappers/source/file_impl.cc
system_wrappers/source/metrics_default.cc
system_wrappers/source/rw_lock_posix.cc

rtc_base/bitbuffer.cc
rtc_base/checks.cc
rtc_base/criticalsection.cc
rtc_base/event.cc
rtc_base/event_tracer.cc
rtc_base/logging.cc
rtc_base/platform_file.cc
rtc_base/platform_thread.cc
rtc_base/race_checker.cc
rtc_base/stringencode.cc

rtc_base/timeutils.cc
rtc_base/string_to_number.cc
common_types.cc

codec/interf_dec.c
codec/interf_enc.c
codec/sp_dec.c
codec/sp_enc.c

webrtc.cc
)

function(prepend_path var prefix)
   set(listVar "")
   foreach(f ${ARGN})
      list(APPEND listVar "${prefix}/${f}")
   endforeach(f)
   set(${var} "${listVar}" PARENT_SCOPE)
endfunction(prepend_path)

prepend_path(WEBRTC_SRC ${CMAKE_CURRENT_SOURCE_DIR}/webrtc ${WEBRTC_SRC_})

add_definitions(-DWEBRTC_APM_DEBUG_DUMP=0)
add_definitions(-DWEBRTC_INTELLIGIBILITY_ENHANCER=0)

add_library(webrtc SHARED ${WEBRTC_SRC})
if (WIN32)
  target_link_libraries(webrtc winmm)
endif ()
#add_executable(webrtc-audio-processing WebRtcAudioTest.cpp)
#target_link_libraries(webrtc-audio-processing webrtc)
target_link_libraries(webrtc)
